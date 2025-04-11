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
    private const string PASSED_NOTIFICATION_TEMPLATE =
        "Chúc mừng! Bạn đã hoàn thành lớp {0}. Bạn đã được chuyển lên cấp độ tiếp theo và đang chờ xếp lớp mới.";

    private const string FAILED_NOTIFICATION_TEMPLATE =
        "Lớp {0} đã kết thúc. Bạn cần cố gắng hơn ở lớp tiếp theo. Bạn đang chờ được xếp lớp mới.";

    private const string INSTRUCTOR_NOTIFICATION_TEMPLATE =
        "Điểm số của lớp {0} đã được công bố cho học viên.";

    // Config keys for passing grades
    private const string LEVEL_PASSING_GRADE_CONFIG_PREFIX = "Điểm yêu cầu của level ";
    private const decimal PASSING_GRADE = 5.0m;
    private const string VIETNAM_TIMEZONE = "+07:00";

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
            var classInfo = await _unitOfWork.ClassRepository.FindSingleAsync(c => c.Id == classId);
            if (classInfo == null)
            {
                throw new ArgumentException("Class not found");
            }

            if (classInfo.Status == ClassStatus.Finished)
            {
                throw new BadRequestException("Scores for this class have already been published.");
            }

            // Load level if needed (outside the loop)
            if (classInfo.Level == null)
            {
                classInfo.Level = await _unitOfWork.LevelRepository.FindSingleAsync(l => l.Id == classInfo.LevelId);
            }

            // Fixed passing grade value of 5.0 as per requirements
            const decimal PASSING_GRADE = 5.0m;

            // Check if there are any students enrolled in this class
            var studentClasses = await _unitOfWork.StudentClassRepository.FindAsync(sc => sc.ClassId == classId);
            if (!studentClasses.Any())
            {
                throw new BadRequestException("No students enrolled in this class.");
            }

            // Log students without GPA
            var studentsWithoutGPA = studentClasses.Where(sc => !sc.GPA.HasValue).ToList();
            if (studentsWithoutGPA.Any())
            {
                Console.WriteLine($"{studentsWithoutGPA.Count} students in class {classId} have no GPA values");
            }

            // Get all student accounts for this class - done once outside the loop
            var studentFirebaseIds = studentClasses.Select(sc => sc.StudentFirebaseId).ToList();
            var studentAccounts = await _unitOfWork.AccountRepository.FindAsync(
                a => studentFirebaseIds.Contains(a.AccountFirebaseId));

            // Create a dictionary for faster lookups
            var studentAccountMap = studentAccounts.ToDictionary(a => a.AccountFirebaseId);

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Update class status
                classInfo.IsScorePublished = true;
                classInfo.Status = ClassStatus.Finished;
                classInfo.UpdateById = account.AccountFirebaseId;
                classInfo.UpdatedAt = DateTime.UtcNow.AddHours(7);

                await _unitOfWork.ClassRepository.UpdateAsync(classInfo);

                // Prepare data for batch operations
                var studentClassesToUpdate = new List<StudentClass>();
                var studentsToUpdate = new List<Account>();
                var notificationTasks = new List<Task>();
                var certificateTasks = new Dictionary<Guid, Task<string>>();

                // Process all student updates first without awaiting
                foreach (var studentClass in studentClasses)
                {
                    if (!studentClass.GPA.HasValue)
                    {
                        Console.WriteLine(
                            $"Student {studentClass.StudentFirebaseId} in class has no GPA value, skipping");
                        continue;
                    }

                    if (!studentAccountMap.TryGetValue(studentClass.StudentFirebaseId, out var student))
                    {
                        Console.WriteLine(
                            $"Student with FirebaseId {studentClass.StudentFirebaseId} not found in database");
                        continue;
                    }

                    // Process each student based on their GPA
                    bool isPassed = studentClass.GPA.Value >= PASSING_GRADE;

                    // Update common properties
                    studentClass.IsPassed = isPassed;
                    studentClass.UpdateById = account.AccountFirebaseId;
                    studentClass.UpdatedAt = DateTime.UtcNow.AddHours(7);

                    if (isPassed)
                    {
                        // Start certificate generation in parallel - don't await yet
                        var certificateTask =
                            _serviceFactory.CertificateService.GenerateCertificateAsync(studentClass.Id);
                        certificateTasks.Add(studentClass.Id, certificateTask);

                        // If there's a next level, update the student's level
                        if (classInfo.Level.NextLevelId.HasValue)
                        {
                            student.LevelId = classInfo.Level.NextLevelId.Value;
                            studentsToUpdate.Add(student);
                        }

                        // Queue notification without awaiting
                        notificationTasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                await _serviceFactory.NotificationService.SendNotificationAsync(
                                    studentClass.StudentFirebaseId,
                                    "Thông báo hoàn thành khóa học",
                                    string.Format(PASSED_NOTIFICATION_TEMPLATE, classInfo.Name)
                                );
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(
                                    $"Error sending passed notification to {studentClass.StudentFirebaseId}: {ex.Message}");
                            }
                        }));
                    }
                    else
                    {
                        // Queue notification without awaiting
                        notificationTasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                await _serviceFactory.NotificationService.SendNotificationAsync(
                                    studentClass.StudentFirebaseId,
                                    "Thông báo kết thúc khóa học",
                                    string.Format(FAILED_NOTIFICATION_TEMPLATE, classInfo.Name)
                                );
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(
                                    $"Error sending failed notification to {studentClass.StudentFirebaseId}: {ex.Message}");
                            }
                        }));
                    }

                    // Add to batch update list rather than updating one by one
                    studentClassesToUpdate.Add(studentClass);
                }

                // Now await all the certificate tasks and update the certificate URLs
                if (certificateTasks.Any())
                {
                    await Task.WhenAll(certificateTasks.Values);

                    // Update certificate URLs
                    foreach (var studentClass in studentClassesToUpdate.Where(sc => sc.IsPassed))
                    {
                        if (certificateTasks.TryGetValue(studentClass.Id, out var task))
                        {
                            studentClass.CertificateUrl = await task;
                        }
                    }
                }

                // Batch update student classes
                var updateClassTasks = studentClassesToUpdate.Select(sc =>
                    _unitOfWork.StudentClassRepository.UpdateAsync(sc));
                await Task.WhenAll(updateClassTasks);

                // Batch update student accounts
                if (studentsToUpdate.Any())
                {
                    var updateStudentTasks = studentsToUpdate.Select(s =>
                        _unitOfWork.AccountRepository.UpdateAsync(s));
                    await Task.WhenAll(updateStudentTasks);
                }

                // Batch update student statuses to WaitingForClass
                var statusUpdateTasks = studentClassesToUpdate.Select(sc =>
                    _serviceFactory.StudentClassService.UpdateStudentStatusAsync(
                        sc.StudentFirebaseId,
                        StudentStatus.WaitingForClass,
                        account
                    ));
                await Task.WhenAll(statusUpdateTasks);

                // Notify instructor if one is assigned to the class
                if (!string.IsNullOrEmpty(classInfo.InstructorId))
                {
                    notificationTasks.Add(Task.Run(async () =>
                    {
                        try
                        {
                            await _serviceFactory.NotificationService.SendNotificationAsync(
                                classInfo.InstructorId,
                                "Điểm đã được công bố",
                                string.Format(INSTRUCTOR_NOTIFICATION_TEMPLATE, classInfo.Name)
                            );
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(
                                $"Error sending notification to instructor {classInfo.InstructorId}: {ex.Message}");
                        }
                    }));
                }

                // Wait for all notification tasks to complete
                await Task.WhenAll(notificationTasks);

                int passedCount = studentClassesToUpdate.Count(sc => sc.IsPassed);
                int failedCount = studentClassesToUpdate.Count(sc => !sc.IsPassed);
                Console.WriteLine(
                    $"Successfully published scores for class {classId}. Passed: {passedCount}, Failed: {failedCount}");
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error publishing scores for class {classId}: {ex.Message}");
            throw;
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
            var classInfo = await _unitOfWork.ClassRepository.FindSingleAsync(c => c.Id == classId);
            if (classInfo == null)
            {
                throw new NotFoundException($"Class with ID {classId} not found");
            }

            if (classInfo.Level == null && classInfo.LevelId != Guid.Empty)
            {
                classInfo.Level = await _unitOfWork.LevelRepository.FindSingleAsync(l => l.Id == classInfo.LevelId);
            }

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

            // Get student information by Firebase IDs
            var studentFirebaseIds = studentClasses.Select(sc => sc.StudentFirebaseId).ToList();
            var students =
                await _unitOfWork.AccountRepository.FindAsync(a => studentFirebaseIds.Contains(a.AccountFirebaseId));
            var studentDict = students.ToDictionary(s => s.AccountFirebaseId);

            // Get all scores for these student-classes
            var studentClassIds = studentClasses.Select(sc => sc.Id).ToList();
            var allScores = await _unitOfWork.StudentClassScoreRepository.FindAsync(
                scs => studentClassIds.Contains(scs.StudentClassId));

            // Get all criteria used
            var criteriaIds = allScores.Select(scs => scs.CriteriaId).Distinct().ToList();
            var allCriteria = await _unitOfWork.CriteriaRepository.FindAsync(c => criteriaIds.Contains(c.Id));
            var criteriaDict = allCriteria.ToDictionary(c => c.Id);

            // Create scores dictionary for quick lookup
            var scoresDict = allScores
                .GroupBy(s => s.StudentClassId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Create view models for each student
            var studentViewModels = new List<StudentScoreViewModel>();

            foreach (var sc in studentClasses)
            {
                // Try to get the student name
                string studentName = "Unknown Student";
                if (studentDict.TryGetValue(sc.StudentFirebaseId, out var student))
                {
                    studentName = student.FullName ?? student.UserName ?? studentName;
                }

                // Get scores for this student
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
    
}