using System.Collections.Concurrent;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.StudentScore;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.PubSub.Notification;
using PhotonPiano.PubSub.Pubsub;
using PhotonPiano.Shared.Exceptions;
using PhotonPiano.Shared.Utils;

namespace PhotonPiano.BusinessLogic.Services;

public class StudentClassScoreService : IStudentClassScoreService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IServiceFactory _serviceFactory;

    private readonly IServiceProvider _serviceProvider;

    // Constants for notification templates
    private const string PassedNotificationTemplate =
        "Congratulations! You have completed the class {0}. You have been moved to the next level and are waiting for class placement.";

    private const string FailedNotificationTemplate =
        "Class {0} has ended. You need to work harder in the next class. You are waiting for new class placement.";

    private const string InstructorNotificationTemplate =
        "Scores for class {0} have been published to students.";

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
                var backgroundJobClient  = _serviceProvider.GetRequiredService<IBackgroundJobClient>();
                backgroundJobClient.Enqueue<CertificateService>(x => x.CronAutoGenerateCertificatesAsync(classId));
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

    private async Task<Dictionary<string, bool>> ValidStudentsAttendance(Guid classId,
        List<StudentClass> studentClasses)
    {
        try {
            // Get attendance results using the existing service method
            var attendanceResults 
                = await _serviceFactory.SlotService.GetAllAttendanceResultByClassId(classId);
            
            var result = attendanceResults.ToDictionary(
                ar => ar.StudentId,
                ar => ar.IsPassed
            );
            
            foreach (var studentClass in studentClasses)
            {
                result.TryAdd(studentClass.StudentFirebaseId, false);
            }
        
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking attendance thresholds: {ex.Message}");
            return studentClasses.ToDictionary(
                sc => sc.StudentFirebaseId,
                _ => false
            );
        }
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
        
        var attendanceResults = await ValidStudentsAttendance(classId, studentClasses);

        // Prepare collections for updates
        var studentClassUpdates = new List<StudentClass>();
        var studentUpdates = new List<Account>();
        var passedStudents = new List<StudentClass>();

        foreach (var studentClass in studentClasses)
        {
            var student = studentAccountMap[studentClass.StudentFirebaseId];
            
            // Get student's attendance status
            bool meetsAttendanceThreshold = attendanceResults.TryGetValue(studentClass.StudentFirebaseId, out bool attendanceStatus) 
                ? attendanceStatus 
                : false;
            
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

    // private async Task ProcessCertificates(List<StudentClass> passedStudents)
    // {
    //     if (!passedStudents.Any()) return;
    //
    //     var now = DateTime.UtcNow.AddHours(7);
    //     foreach (var studentClass in passedStudents)
    //     {
    //         try
    //         {
    //             if (studentClass.GPA.HasValue && studentClass.IsPassed)
    //             {
    //                 using (var scope = _serviceProvider.CreateScope())
    //                 {
    //                     var serviceFactory = scope.ServiceProvider.GetRequiredService<IServiceFactory>();
    //                     // var certificateService = serviceFactory.CertificateService;
    //                     // var eligibilityResult = await certificateService.CheckCertificateEligibilityAsync(studentClass.Id);
    //                     //
    //                     // if (eligibilityResult.IsEligible)
    //                     // {
    //                     //     // Only generate certificate if student is eligible
    //                     //     var certificateUrl = await certificateService.GenerateCertificateAsync(studentClass.Id);
    //                     //
    //                     //     if (!string.IsNullOrEmpty(certificateUrl))
    //                     //     {
    //                     //         var updateUnitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    //                     //         await updateUnitOfWork.ExecuteInTransactionAsync(async () =>
    //                     //         {
    //                     //             var sc = await updateUnitOfWork.StudentClassRepository.FindSingleAsync(x =>
    //                     //                 x.Id == studentClass.Id);
    //                     //             if (sc != null)
    //                     //             {
    //                     //                 sc.CertificateUrl = certificateUrl;
    //                     //                 sc.UpdatedAt = now;
    //                     //                 await updateUnitOfWork.StudentClassRepository.UpdateAsync(sc);
    //                     //                 await updateUnitOfWork.SaveChangesAsync();
    //                     //             }
    //                     //         });
    //                     //     }
    //                     // }
    //                     var certificateUrl = await serviceFactory.CertificateService
    //                         .GenerateCertificateAsync(studentClass.Id);
    //                     
    //                     if (string.IsNullOrEmpty(certificateUrl))
    //                         continue;
    //                     var updateUnitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    //                     await updateUnitOfWork.ExecuteInTransactionAsync(async () =>
    //                     {
    //                         var sc = await updateUnitOfWork.StudentClassRepository.FindSingleAsync(x =>
    //                             x.Id == studentClass.Id);
    //                         if (sc != null)
    //                         {
    //                             sc.CertificateUrl = certificateUrl;
    //                             sc.UpdatedAt = now;
    //                             await updateUnitOfWork.StudentClassRepository.UpdateAsync(sc);
    //                             await updateUnitOfWork.SaveChangesAsync();
    //                         }
    //                     });
    //                 }
    //             }
    //         }
    //         catch (Exception ex)
    //         {
    //             Console.WriteLine($"Error processing certificate for student {studentClass.Id}: {ex.Message}");
    //             // Continue with next student
    //         }
    //     }
    // }

    private async Task SendClassCompletionNotifications(List<StudentClass> studentClasses, Class classInfo)
    {
        try
        {
            // First, prepare all notification data without sending
            var notifications = new List<(string recipientId, string title, string content)>();

            foreach (var studentClass in studentClasses.Where(sc => sc.GPA.HasValue))
            {
                bool isPassed = studentClass.IsPassed;
                var title = isPassed ? "Class Completion Notification" : "Class End Notification";
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

            using (var scope = _serviceProvider.CreateScope())
            {
                var pubSubService = scope.ServiceProvider.GetRequiredService<IPubSubService>();
            
                // Publish a message about score publication
                await pubSubService.PublishAsync(new string[] { "score", "published" }, 
                    System.Text.Json.JsonSerializer.Serialize(new {
                        ClassId = classInfo.Id,
                        ClassName = classInfo.Name,
                        PublishedAt = DateTime.UtcNow
                    })
                );
            }
            
            int batchSize = 5;
            for (int i = 0; i < notifications.Count; i += batchSize)
            {
                var batch = notifications.Skip(i).Take(batchSize).ToList();

                // Process each recipient one at a time (avoid parallel processing with shared DbContext)
                foreach (var (recipientId, title, content) in batch)
                {
                    try
                    {
                        await _serviceFactory.NotificationService.SendNotificationAsync(recipientId, title, content);
                        var studentClass = studentClasses.FirstOrDefault(sc => sc.StudentFirebaseId == recipientId);
                        var isPassed = studentClass?.IsPassed ?? true;
                        // Send SignalR notification to specific user with class completion status
                       
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending notification to {recipientId}: {ex.Message}");
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
}