using Microsoft.EntityFrameworkCore;
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

    // Constants for notification templates
    private const string PassedNotificationTemplate =
        "Chúc mừng! Bạn đã hoàn thành lớp {0}. Bạn đã được chuyển lên cấp độ tiếp theo và đang chờ xếp lớp mới.";

    private const string FailedNotificationTemplate =
        "Lớp {0} đã kết thúc. Bạn cần cố gắng hơn ở lớp tiếp theo. Bạn đang chờ được xếp lớp mới.";

    private const string InstructorNotificationTemplate =
        "Điểm số của lớp {0} đã được công bố cho học viên.";

    // Config keys for passing grades
    private const decimal DefaultPassingGrade = 5.0m;

    public StudentClassScoreService(IUnitOfWork unitOfWork, IServiceFactory serviceFactory)
    {
        _unitOfWork = unitOfWork;
        _serviceFactory = serviceFactory;
    }


    public async Task PublishScore(Guid classId, AccountModel account)
    {
        if (classId == Guid.Empty)
        {
            throw new ArgumentException("Invalid class ID", nameof(classId));
        }

        try
        {
            // Load all required data at once to minimize database round trips
            var classInfo = await _unitOfWork.ClassRepository.FindSingleAsync(c =>
                c.Id == classId && c.Status != ClassStatus.Finished);

            if (classInfo == null)
            {
                throw new NotFoundException("Class not found or has already been finished");
            }
            var currentDate = DateTime.UtcNow.AddHours(7); // Vietnam time
            // if (classInfo.Slots..HasValue && classInfo.EndDate.Value > currentDate)
            // {
            //     throw new BadRequestException($"Cannot publish scores: The class has not ended yet. Class end date is {classInfo.EndDate.Value.ToString("dd/MM/yyyy")}.");
            // }
            // Preload level with class to avoid separate query
            if (classInfo.Level == null)
            {
                classInfo.Level = await _unitOfWork.LevelRepository.FindSingleAsync(l => l.Id == classInfo.LevelId);
                if (classInfo.Level == null)
                {
                    throw new NotFoundException($"Level not found for class {classId}");
                }
            }

            // Get all students in this class in one query
            var studentClasses = await _unitOfWork.StudentClassRepository.FindAsync(sc => sc.ClassId == classId);
            if (!studentClasses.Any())
            {
                throw new BadRequestException("No students enrolled in this class.");
            }

            // Get all student accounts in one query for better performance
            var studentFirebaseIds = studentClasses.Select(sc => sc.StudentFirebaseId).Distinct().ToList();
            var studentAccounts =
                await _unitOfWork.AccountRepository.FindAsync(a => studentFirebaseIds.Contains(a.AccountFirebaseId));
            var studentAccountMap = studentAccounts.ToDictionary(a => a.AccountFirebaseId);

            var now = DateTime.UtcNow.AddHours(7); // Vietnam time

            // Prepare collections for batch updates
            var studentClassUpdates = new List<StudentClass>();
            var studentUpdates = new List<Account>();
            var passedStudents = new List<StudentClass>();

            // Process all students in memory to minimize DB interactions
            foreach (var studentClass in studentClasses)
            {
                if (!studentClass.GPA.HasValue)
                {
                    Console.WriteLine($"Student {studentClass.StudentFirebaseId} in class has no GPA value, skipping");
                    continue;
                }

                if (!studentAccountMap.TryGetValue(studentClass.StudentFirebaseId, out var student))
                {
                    Console.WriteLine(
                        $"Student with FirebaseId {studentClass.StudentFirebaseId} not found in database");
                    continue;
                }

                // Update student class data
                bool isPassed = studentClass.GPA.Value >= DefaultPassingGrade;
                studentClass.IsPassed = isPassed;
                studentClass.UpdateById = account.AccountFirebaseId;
                studentClass.UpdatedAt = now;
                studentClassUpdates.Add(studentClass);

                if (isPassed)
                {
                    passedStudents.Add(studentClass);
                }

                // Update student level if passed
                if (isPassed && classInfo.Level.NextLevelId.HasValue)
                {
                    student.LevelId = classInfo.Level.NextLevelId.Value;
                    student.StudentStatus = StudentStatus.WaitingForClass;
                    student.UpdatedAt = now;
                }
                else
                {
                    // Still update status for failed students
                    student.StudentStatus = StudentStatus.WaitingForClass;
                    student.UpdatedAt = now;
                }

                // Track student for update
                studentUpdates.Add(student);
            }

            // Update class status
            classInfo.IsScorePublished = true;
            classInfo.Status = ClassStatus.Finished;
            classInfo.UpdateById = account.AccountFirebaseId;
            classInfo.UpdatedAt = now;

            // Execute all updates in a single transaction for consistency
            var strategy = _unitOfWork.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                // Start a new transaction
                using var transaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    // Update class first
                    await _unitOfWork.ClassRepository.UpdateAsync(classInfo);

                    // Update student classes and accounts in optimized batches
                    await UpdateStudentClassesInBatch(studentClassUpdates);
                    await UpdateStudentAccountsInBatch(studentUpdates);

                    // Commit the transaction
                    await transaction.CommitAsync();
                }
                catch
                {
                    // Rollback on error
                    await transaction.RollbackAsync();
                    throw;
                }
            });

            // Process certificates sequentially after the main transaction
            if (passedStudents.Any())
            {
                await ProcessCertificatesSequentially(passedStudents);
            }

            // Send notifications sequentially
            await SendClassCompletionNotificationsSequentially(studentClassUpdates, classInfo);

            int passedCount = studentClassUpdates.Count(sc => sc.IsPassed);
            int failedCount = studentClassUpdates.Count(sc => !sc.IsPassed);
            Console.WriteLine(
                $"Successfully published scores for class {classId}. Passed: {passedCount}, Failed: {failedCount}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error publishing scores for class {classId}: {ex.Message}");
            throw;
        }
    }

    // Sequential notification method
    private async Task SendClassCompletionNotificationsSequentially(List<StudentClass> studentClasses, Class classInfo)
    {
        try
        {
            // Process notifications one by one
            foreach (var studentClass in studentClasses)
            {
                try
                {
                    bool isPassed = studentClass.IsPassed;
                    var title = isPassed ? "Thông báo hoàn thành khóa học" : "Thông báo kết thúc khóa học";
                    var content = isPassed
                        ? string.Format(PassedNotificationTemplate, classInfo.Name)
                        : string.Format(FailedNotificationTemplate, classInfo.Name);

                    await _serviceFactory.NotificationService.SendNotificationAsync(
                        studentClass.StudentFirebaseId,
                        title,
                        content
                    );

                    // Add a small delay between notifications to reduce database pressure
                    await Task.Delay(50);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Error sending notification to student {studentClass.StudentFirebaseId}: {ex.Message}");
                    // Continue with next student even if this one fails
                }
            }

            // Send notification to instructor
            if (!string.IsNullOrEmpty(classInfo.InstructorId))
            {
                try
                {
                    await _serviceFactory.NotificationService.SendNotificationAsync(
                        classInfo.InstructorId,
                        "Điểm đã được công bố",
                        string.Format(InstructorNotificationTemplate, classInfo.Name)
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Error sending notification to instructor {classInfo.InstructorId}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            // Log but don't rethrow - notifications are not critical
            Console.WriteLine($"Error sending notifications: {ex.Message}");
        }
    }

    private async Task UpdateStudentClassesInBatch(List<StudentClass> studentClasses)
    {
        if (!studentClasses.Any()) return;

        const int BATCH_SIZE = 50;

        for (int i = 0; i < studentClasses.Count; i += BATCH_SIZE)
        {
            var batch = studentClasses.Skip(i).Take(BATCH_SIZE).ToList();
            foreach (var item in batch)
            {
                await _unitOfWork.StudentClassRepository.UpdateAsync(item);
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }

    private async Task UpdateStudentAccountsInBatch(List<Account> students)
    {
        if (!students.Any()) return;

        const int BATCH_SIZE = 50;

        for (int i = 0; i < students.Count; i += BATCH_SIZE)
        {
            var batch = students.Skip(i).Take(BATCH_SIZE).ToList();

            foreach (var item in batch)
            {
                await _unitOfWork.AccountRepository.UpdateAsync(item);
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }

    private async Task ProcessCertificatesSequentially(List<StudentClass> passedStudents)
    {
        if (!passedStudents.Any()) return;

        var now = DateTime.UtcNow.AddHours(7);

        // Process certificates one by one
        foreach (var studentClass in passedStudents)
        {
            try
            {
                // Check if the student is eligible for a certificate directly here
                // instead of relying on CertificateService.IsEligibleForCertificateAsync
                if (studentClass.GPA.HasValue && studentClass.IsPassed)
                {
                    // Generate certificate
                    var certificateUrl =
                        await _serviceFactory.CertificateService.GenerateCertificateAsync(studentClass.Id);

                    if (certificateUrl != null)
                    {
                        // Update certificate URL in database
                        var strategy = _unitOfWork.Database.CreateExecutionStrategy();
                        await strategy.ExecuteAsync(async () =>
                        {
                            using var transaction = await _unitOfWork.BeginTransactionAsync();
                            try
                            {
                                var sc = await _unitOfWork.StudentClassRepository.FindSingleAsync(x =>
                                    x.Id == studentClass.Id);
                                if (sc != null)
                                {
                                    sc.CertificateUrl = certificateUrl;
                                    sc.UpdatedAt = now;
                                    await _unitOfWork.StudentClassRepository.UpdateAsync(sc);
                                    await _unitOfWork.SaveChangesAsync();
                                }

                                await transaction.CommitAsync();
                            }
                            catch
                            {
                                await transaction.RollbackAsync();
                                throw;
                            }
                        });
                    }
                }
                else
                {
                    Console.WriteLine(
                        $"Student {studentClass.Id} is not eligible for a certificate. GPA: {studentClass.GPA}, IsPassed: {studentClass.IsPassed}");
                }

                // Add a small delay between certificate generations to reduce database pressure
                await Task.Delay(100);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating certificate for student {studentClass.Id}: {ex.Message}");
                // Continue with next student even if this one fails
            }
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