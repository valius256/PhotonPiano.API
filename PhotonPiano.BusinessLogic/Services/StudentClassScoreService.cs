using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.StudentScore;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services;

public class StudentClassScoreService : IStudentClassScoreService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IServiceFactory _serviceFactory;

    private readonly IServiceProvider _serviceProvider;

    // Constants for notification templates
    private const string PassedNotificationTemplate =
        "Chúc mừng! Bạn đã hoàn thành lớp {0}. Bạn đã được chuyển lên cấp độ tiếp theo và đang chờ xếp lớp mới.";

    private const string FailedNotificationTemplate =
        "Lớp {0} đã kết thúc. Bạn cần cố gắng hơn ở lớp tiếp theo. Bạn đang chờ được xếp lớp mới.";

    private const string InstructorNotificationTemplate =
        "Điểm số của lớp {0} đã được công bố cho học viên.";

    private const decimal DefaultPassingGrade = 5.0m;
    private const int BatchSize = 50;

    public StudentClassScoreService(IUnitOfWork unitOfWork, IServiceFactory serviceFactory,
        IServiceProvider serviceProvider)
    {
        _unitOfWork = unitOfWork;
        _serviceFactory = serviceFactory;
        _serviceProvider = serviceProvider;
    }


    public async Task PublishScore(Guid classId, AccountModel account)
    {
        if (classId == Guid.Empty)
        {
            throw new ArgumentException("Invalid class ID", nameof(classId));
        }

        if (account == null || string.IsNullOrEmpty(account.AccountFirebaseId))
        {
            throw new ArgumentException("Invalid account information", nameof(account));
        }

        try
        {
            var classInfo = await ValidateAndGetClass(classId);
            var now = DateTime.UtcNow.AddHours(7);

            var (studentClasses, studentUpdates, passedStudents) =
                await PrepareStudentDataForScorePublishing(classId, classInfo, account, now);

            // Update class status
            classInfo.IsScorePublished = true;
            classInfo.Status = ClassStatus.Finished;
            classInfo.UpdateById = account.AccountFirebaseId;
            classInfo.UpdatedAt = now;

            // Execute all database operations in a single transaction
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Update class first
                await _unitOfWork.ClassRepository.UpdateAsync(classInfo);

                // Update student classes and accounts in batches
                await UpdateEntitiesInBatches(studentClasses, _unitOfWork.StudentClassRepository.UpdateAsync);
                await UpdateEntitiesInBatches(studentUpdates, _unitOfWork.AccountRepository.UpdateAsync);
            });

            if (passedStudents.Any())
            {
                await ProcessCertificates(passedStudents);
            }

            await SendClassCompletionNotifications(studentClasses, classInfo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error publishing scores for class {classId}: {ex.Message}");
            throw;
        }
    }

    private async Task<Class> ValidateAndGetClass(Guid classId)
    {
        var classInfo = await _unitOfWork.ClassRepository.FindSingleAsync(c => c.Id == classId);
        if (classInfo == null)
        {
            throw new NotFoundException("Class not found");
        }

        switch (classInfo.Status)
        {
            case ClassStatus.Ongoing:
                break;
            case ClassStatus.Finished:
                throw new BadRequestException("Cannot publish scores: The class has already been finished.");
            case ClassStatus.NotStarted:
                throw new BadRequestException("Cannot publish scores: The class has not started yet.");
            default:
                throw new BadRequestException(
                    $"Cannot publish scores: The class status ({classInfo.Status}) is invalid for publishing scores.");
        }

        // Load level if needed
        if (classInfo.Level == null && classInfo.LevelId != Guid.Empty)
        {
            classInfo.Level = await _unitOfWork.LevelRepository.FindSingleAsync(l => l.Id == classInfo.LevelId);
            if (classInfo.Level == null)
            {
                throw new NotFoundException($"Level not found for class {classId}");
            }
        }

        return classInfo;
    }

    private async Task<(List<StudentClass> StudentClasses, List<Account> StudentUpdates, List<StudentClass>
            PassedStudents)>
        PrepareStudentDataForScorePublishing(Guid classId, Class classInfo, AccountModel account,
            DateTime updateTime)
    {
        var studentClasses = await _unitOfWork.StudentClassRepository
            .FindAsync(sc => sc.ClassId == classId);

        if (!studentClasses.Any())
        {
            throw new BadRequestException("No students enrolled in this class.");
        }

        // Get all student accounts in one query for better performance
        var studentFirebaseIds = studentClasses.Select(sc => sc.StudentFirebaseId).Distinct().ToList();
        var studentAccounts = await _unitOfWork.AccountRepository
            .FindAsync(a => studentFirebaseIds.Contains(a.AccountFirebaseId));
        var studentAccountMap = studentAccounts.ToDictionary(a => a.AccountFirebaseId);

        // Validate that all students have GPA values before proceeding
        var studentsWithoutGpa = studentClasses.Where(sc => !sc.GPA.HasValue).ToList();
        if (studentsWithoutGpa.Any())
        {
            var missingGpaCount = studentsWithoutGpa.Count;
            throw new BadRequestException(
                $"Cannot publish scores: {missingGpaCount} student(s) are missing GPA values. Please ensure all students have been graded.");
        }

        // Validate that all students exist in the database
        var missingStudents = studentClasses
            .Select(sc => sc.StudentFirebaseId)
            .Except(studentAccountMap.Keys)
            .ToList();


        if (missingStudents.Any())
        {
            throw new BadRequestException(
                $"Cannot publish scores: {missingStudents.Count} student(s) could not be found in the database. Please refresh the page and try again.");
        }


        // Prepare collections for updates
        var studentClassUpdates = new List<StudentClass>();
        var studentUpdates = new List<Account>();
        var passedStudents = new List<StudentClass>();

        foreach (var studentClass in studentClasses)
        {
            var student = studentAccountMap[studentClass.StudentFirebaseId];
            // Update student class data
            var isPassed = studentClass.GPA.Value >= DefaultPassingGrade;
            studentClass.IsPassed = isPassed;
            studentClass.UpdateById = account.AccountFirebaseId;
            studentClass.UpdatedAt = updateTime;
            studentClassUpdates.Add(studentClass);

            if (isPassed)
            {
                passedStudents.Add(studentClass);
            }

            // Update student level if passed
            if (isPassed && classInfo.Level?.NextLevelId.HasValue == true)
            {
                student.LevelId = classInfo.Level.NextLevelId.Value;
                student.StudentStatus = StudentStatus.WaitingForClass;
                student.UpdatedAt = updateTime;
            }
            else
            {
                // Still update status for failed students
                student.StudentStatus = StudentStatus.WaitingForClass;
                student.UpdatedAt = updateTime;
            }

            studentUpdates.Add(student);
        }

        return (studentClassUpdates, studentUpdates, passedStudents);
    }

    private async Task UpdateEntitiesInBatches<T>(List<T> entities, Func<T, Task> updateFunc) where T : class
    {
        if (!entities.Any()) return;

        for (int i = 0; i < entities.Count; i += BatchSize)
        {
            var batch = entities.Skip(i).Take(BatchSize).ToList();
            var updateTasks = batch.Select(updateFunc);
            await Task.WhenAll(updateTasks);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    private async Task ProcessCertificates(List<StudentClass> passedStudents)
    {
        if (!passedStudents.Any()) return;

        var now = DateTime.UtcNow.AddHours(7);
        foreach (var studentClass in passedStudents)
        {
            try
            {
                if (studentClass.GPA.HasValue && studentClass.IsPassed)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var serviceFactory = scope.ServiceProvider.GetRequiredService<IServiceFactory>();
                        var certificateUrl = await serviceFactory.CertificateService
                            .GenerateCertificateAsync(studentClass.Id);

                        if (string.IsNullOrEmpty(certificateUrl))
                            continue;
                        var updateUnitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        await updateUnitOfWork.ExecuteInTransactionAsync(async () =>
                        {
                            var sc = await updateUnitOfWork.StudentClassRepository.FindSingleAsync(x =>
                                x.Id == studentClass.Id);
                            if (sc != null)
                            {
                                sc.CertificateUrl = certificateUrl;
                                sc.UpdatedAt = now;
                                await updateUnitOfWork.StudentClassRepository.UpdateAsync(sc);
                                await updateUnitOfWork.SaveChangesAsync();
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing certificate for student {studentClass.Id}: {ex.Message}");
                // Continue with next student
            }
        }
    }
    
    private async Task SendClassCompletionNotifications(List<StudentClass> studentClasses, Class classInfo)
    {
        try
        {
            // First, prepare all notification data without sending
            var notifications = new List<(string recipientId, string title, string content)>();

            foreach (var studentClass in studentClasses.Where(sc => sc.GPA.HasValue))
            {
                bool isPassed = studentClass.IsPassed;
                var title = isPassed ? "Thông báo hoàn thành khóa học" : "Thông báo kết thúc khóa học";
                var content = isPassed
                    ? string.Format(PassedNotificationTemplate, classInfo.Name)
                    : string.Format(FailedNotificationTemplate, classInfo.Name);

                notifications.Add((studentClass.StudentFirebaseId, title, content));
            }

            // Add instructor notification if an instructor exists
            if (!string.IsNullOrEmpty(classInfo.InstructorId))
            {
                notifications.Add((
                    classInfo.InstructorId,
                    "Điểm đã được công bố",
                    string.Format(InstructorNotificationTemplate, classInfo.Name)
                ));
            }

            // Process notifications in batches using a separate scope for each batch
            int batchSize = 5;
            for (int i = 0; i < notifications.Count; i += batchSize)
            {
                var batch = notifications.Skip(i).Take(batchSize).ToList();

                // Process each recipient one at a time (avoid parallel processing with shared DbContext)
                foreach (var (recipientId, title, content) in batch)
                {
                    try
                    {
                        // Use the notification service to send the notification
                        // This should internally create its own DbContext scope
                        await _serviceFactory.NotificationService.SendNotificationAsync(recipientId, title, content);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending notification to {recipientId}: {ex.Message}");
                        // Continue with the next notification even if this one fails
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending notifications: {ex.Message}");
            // Log but don't rethrow - we don't want notification failures to roll back the transaction
        }
    }

    public async Task<ClassScoreViewModel> GetClassScoresWithCriteria(Guid classId)
    {
        if (classId == Guid.Empty)
        {
            throw new ArgumentException("Invalid class ID", nameof(classId));
        }

        try
        {
            // Load class info with level in one query if possible
            var classInfo = await _unitOfWork.ClassRepository.FindSingleAsync(c => c.Id == classId);
            if (classInfo == null)
            {
                throw new NotFoundException($"Class with ID {classId} not found");
            }

            if (classInfo.Level == null && classInfo.LevelId != Guid.Empty)
            {
                classInfo.Level = await _unitOfWork.LevelRepository.FindSingleAsync(l => l.Id == classInfo.LevelId);
            }

            // Get all student classes in one query
            var studentClasses = await _unitOfWork.StudentClassRepository.FindAsync(sc => sc.ClassId == classId);

            if (!studentClasses.Any())
            {
                return new ClassScoreViewModel
                {
                    ClassId = classId,
                    ClassName = classInfo.Name,
                    LevelName = classInfo.Level?.Name ?? "Unknown Level",
                    IsScorePublished = classInfo.IsScorePublished,
                    Students = new List<StudentScoreViewModel>()
                };
            }

            // Get student info in one query
            var studentFirebaseIds = studentClasses.Select(sc => sc.StudentFirebaseId).Distinct().ToList();
            var students =
                await _unitOfWork.AccountRepository.FindAsync(a => studentFirebaseIds.Contains(a.AccountFirebaseId));
            var studentDict = students.ToDictionary(s => s.AccountFirebaseId);

            // Get all scores in one query
            var studentClassIds = studentClasses.Select(sc => sc.Id).ToList();
            var allScores = await _unitOfWork.StudentClassScoreRepository.FindAsync(
                scs => studentClassIds.Contains(scs.StudentClassId));

            // Get all criteria in one query
            var criteriaIds = allScores.Select(scs => scs.CriteriaId).Distinct().ToList();
            var allCriteria = await _unitOfWork.CriteriaRepository.FindAsync(c => criteriaIds.Contains(c.Id));

            // Create dictionary for quick lookups
            var criteriaDict = allCriteria.ToDictionary(c => c.Id);

            // Group scores by student class ID for faster lookup
            var scoresDict = allScores
                .GroupBy(s => s.StudentClassId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Create student view models
            var studentViewModels = new List<StudentScoreViewModel>();

            foreach (var sc in studentClasses)
            {
                // Get student name
                string studentName = "Unknown Student";
                if (studentDict.TryGetValue(sc.StudentFirebaseId, out var student))
                {
                    studentName = student.FullName ?? student.UserName ?? studentName;
                }

                // Get criteria scores for this student
                var criteriaScores = new List<CriteriaScoreViewModel>();
                if (scoresDict.TryGetValue(sc.Id, out var studentScores))
                {
                    foreach (var score in studentScores)
                    {
                        if (criteriaDict.TryGetValue(score.CriteriaId, out var criteria))
                        {
                            criteriaScores.Add(new CriteriaScoreViewModel
                            {
                                CriteriaId = score.CriteriaId,
                                CriteriaName = criteria.Name,
                                Weight = criteria.Weight,
                                Score = score.Score
                            });
                        }
                        else
                        {
                            criteriaScores.Add(new CriteriaScoreViewModel
                            {
                                CriteriaId = score.CriteriaId,
                                CriteriaName = "Unknown Criteria",
                                Weight = 0,
                                Score = score.Score
                            });
                        }
                    }

                    // Sort criteria by weight as requested
                    criteriaScores = criteriaScores.OrderBy(cs => cs.Weight).ToList();
                }

                studentViewModels.Add(new StudentScoreViewModel
                {
                    StudentId = sc.Id.ToString(),
                    StudentName = studentName,
                    Gpa = sc.GPA,
                    IsPassed = sc.IsPassed,
                    InstructorComment = sc.InstructorComment,
                    CertificateUrl = sc.CertificateUrl,
                    CriteriaScores = criteriaScores
                });
            }

            // Return the complete view model
            return new ClassScoreViewModel
            {
                ClassId = classId,
                ClassName = classInfo.Name,
                LevelName = classInfo.Level?.Name ?? "Unknown Level",
                IsScorePublished = classInfo.IsScorePublished,
                Students = studentViewModels
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting scores for class {classId}: {ex.Message}");
            throw;
        }
    }

    // Helper method to update student statuses in bulk
    private async Task BulkUpdateStudentStatusAsync(List<string> studentFirebaseIds, StudentStatus newStatus,
        AccountModel account)
    {
        if (!studentFirebaseIds.Any())
            return;

        var now = DateTime.UtcNow.AddHours(7);
        var students =
            await _unitOfWork.AccountRepository.FindAsync(a => studentFirebaseIds.Contains(a.AccountFirebaseId));

        foreach (var student in students)
        {
            student.StudentStatus = newStatus;
            student.UpdatedAt = now;
            await _unitOfWork.AccountRepository.UpdateAsync(student);
        }
    }

    public async Task UnpublishScore(Guid classId, AccountModel account)
    {
        if (classId == Guid.Empty)
        {
            throw new ArgumentException("Invalid class ID", nameof(classId));
        }

        try
        {
            var classInfo = await _unitOfWork.ClassRepository.FindSingleAsync(c => c.Id == classId);
            if (classInfo == null)
            {
                throw new ArgumentException("Class not found");
            }

            if (!classInfo.IsScorePublished || classInfo.Status != ClassStatus.Finished)
            {
                throw new BadRequestException("Scores for this class have not been published yet.");
            }

            // Load level if needed
            if (classInfo.Level == null)
            {
                classInfo.Level = await _unitOfWork.LevelRepository.FindSingleAsync(l => l.Id == classInfo.LevelId);
            }

            // Check if there are any students enrolled in this class
            var studentClasses = await _unitOfWork.StudentClassRepository.FindAsync(sc => sc.ClassId == classId);
            if (!studentClasses.Any())
            {
                throw new BadRequestException("No students enrolled in this class.");
            }

            // Get all student accounts for this class - done once outside the loop
            var studentFirebaseIds = studentClasses.Select(sc => sc.StudentFirebaseId).ToList();
            var studentAccounts = await _unitOfWork.AccountRepository.FindAsync(
                a => studentFirebaseIds.Contains(a.AccountFirebaseId));

            // Create a dictionary for faster lookups - this keeps data in RAM for quick access
            var studentAccountMap = studentAccounts.ToDictionary(a => a.AccountFirebaseId);

            // Prepare data structures for in-memory processing
            var studentClassesToUpdate = new List<StudentClass>();
            var studentsToUpdate = new List<Account>();
            var studentIdsForStatusUpdate = new List<string>();
            var now = DateTime.UtcNow.AddHours(7); // Vietnam time

            classInfo.Status = ClassStatus.Ongoing;
            await _unitOfWork.SaveChangesAsync();
            // First, process all data in memory
            foreach (var studentClass in studentClasses)
            {
                if (!studentAccountMap.TryGetValue(studentClass.StudentFirebaseId, out var student))
                {
                    Console.WriteLine(
                        $"Student with FirebaseId {studentClass.StudentFirebaseId} not found in database");
                    continue;
                }

                // If the student passed and was moved to the next level, revert that change
                if (studentClass.IsPassed && classInfo.Level.NextLevelId.HasValue)
                {
                    // Check if student's current level is the next level of the class's level
                    if (student.LevelId == classInfo.Level.NextLevelId.Value)
                    {
                        // Revert back to the class's level
                        student.LevelId = classInfo.LevelId;
                        student.UpdatedAt = now;
                        studentsToUpdate.Add(student);
                    }
                }

                // Reset certificate and pass status
                studentClass.IsPassed = false;
                studentClass.CertificateUrl = null;
                studentClass.UpdateById = account.AccountFirebaseId;
                studentClass.UpdatedAt = now;

                // Add to batch update list
                studentClassesToUpdate.Add(studentClass);

                // Add to the list for status update
                studentIdsForStatusUpdate.Add(studentClass.StudentFirebaseId);
            }

            // Now execute the transaction with all in-memory prepared data
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Update class status to active
                classInfo.IsScorePublished = false;
                classInfo.UpdateById = account.AccountFirebaseId;
                classInfo.UpdatedAt = now;

                await _unitOfWork.ClassRepository.UpdateAsync(classInfo);

                // Batch update student classes
                foreach (var sc in studentClassesToUpdate)
                {
                    await _unitOfWork.StudentClassRepository.UpdateAsync(sc);
                }

                // Batch update student accounts level changes
                foreach (var s in studentsToUpdate)
                {
                    await _unitOfWork.AccountRepository.UpdateAsync(s);
                }

                // Update student statuses back to InClass
                await BulkUpdateStudentStatusAsync(studentIdsForStatusUpdate, StudentStatus.InClass, account);
            });

            // Send notifications outside the main transaction
            var notificationTasks = new List<Task>();

            // Student notifications
            foreach (var studentClass in studentClassesToUpdate)
            {
                notificationTasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        await _serviceFactory.NotificationService.SendNotificationAsync(
                            studentClass.StudentFirebaseId,
                            "Thông báo thu hồi kết quả",
                            $"Kết quả của lớp {classInfo.Name} đã được thu hồi để đánh giá lại. Bạn vẫn đang tham gia lớp này."
                        );
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(
                            $"Error sending unpublish notification to {studentClass.StudentFirebaseId}: {ex.Message}");
                    }
                }));
            }

            // Instructor notification
            if (!string.IsNullOrEmpty(classInfo.InstructorId))
            {
                notificationTasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        await _serviceFactory.NotificationService.SendNotificationAsync(
                            classInfo.InstructorId,
                            "Thông báo thu hồi kết quả",
                            $"Kết quả của lớp {classInfo.Name} đã được thu hồi để đánh giá lại."
                        );
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(
                            $"Error sending unpublish notification to instructor {classInfo.InstructorId}: {ex.Message}");
                    }
                }));
            }

            // Wait for all notifications to complete
            await Task.WhenAll(notificationTasks);

            Console.WriteLine($"Successfully unpublished scores for class {classId}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error unpublishing scores for class {classId}: {ex.Message}");
            throw;
        }
    }
}