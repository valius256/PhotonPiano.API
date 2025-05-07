using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.StudentScore;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.PubSub.StudentClassScore;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services;

public class StudentClassScoreService : IStudentClassScoreService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IServiceFactory _serviceFactory;

    private readonly IServiceProvider _serviceProvider;

    private readonly ILogger<ServiceFactory> _logger;

    // Constants for notification templates
    private const string PassedNotificationTemplate =
        "Congratulations! You have completed the class {0}. You have been moved to the next level and are waiting for class placement.";

    private const string FailedNotificationTemplate =
        "Class {0} has ended. You need to work harder in the next class. You are waiting for new class placement.";

    private const string InstructorNotificationTemplate =
        "Scores for class {0} have been published to students.";

    private const decimal DefaultPassingGrade = 5.0m;
    public StudentClassScoreService(IUnitOfWork unitOfWork, IServiceFactory serviceFactory,
        IServiceProvider serviceProvider, ILogger<ServiceFactory> logger)
    {
        _unitOfWork = unitOfWork;
        _serviceFactory = serviceFactory;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }


    public async Task PublishScore(Guid classId, AccountModel account)
    {
        if (classId == Guid.Empty)
        {
            throw new ArgumentException("Invalid class ID", nameof(classId));
        }

        try
        {
            var classInfo = await ValidateAndGetClass(classId);
            var now = DateTime.UtcNow.AddHours(7);


            await _unitOfWork.SlotStudentRepository.ExecuteUpdateAsync(
                x => x.Slot.ClassId == classId && x.AttendanceStatus == AttendanceStatus.NotYet,
                calls => calls.SetProperty(x => x.AttendanceStatus, AttendanceStatus.Attended)
            );

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
                await _unitOfWork.ClassRepository.UpdateAsync(classInfo);
                await _unitOfWork.StudentClassRepository.UpdateRangeAsync(studentClasses);
                await _unitOfWork.AccountRepository.UpdateRangeAsync(studentUpdates);
            });

            if (passedStudents.Any())
            {
                var backgroundJobClient = _serviceProvider.GetRequiredService<IBackgroundJobClient>();
                backgroundJobClient.Enqueue<CertificateService>(x => x.AutoGenerateCertificatesAsync(classId));
            }
            
            await SendClassCompletionNotifications(studentClasses, classInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing scores for class {ClassId}", classId);
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
        
        if (classInfo.LevelId == Guid.Empty)
        {
            throw new BadRequestException("Cannot publish scores: The class has no associated level.");
        }
        
        if (classInfo.Level == null)
        {
            classInfo.Level = await _unitOfWork.LevelRepository.FindSingleAsync(l => l.Id == classInfo.LevelId);
            if (classInfo.Level == null)
            {
                throw new NotFoundException($"Level with ID {classInfo.LevelId} not found. Cannot proceed with score publishing.");
            }
        }
        
        switch (classInfo.Status)
        {
            case ClassStatus.Finished:
                break;
            case ClassStatus.Ongoing:
                throw new BadRequestException("Cannot publish scores: The class has not finished.");
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

    // private async Task<Dictionary<string, bool>> ValidStudentsAttendance(Guid classId,
    //     List<StudentClass> studentClasses)
    // {
    //     try
    //     {
    //         // Get attendance results using the existing service method
    //         var attendanceResults
    //             = await _serviceFactory.SlotService.GetAllAttendanceResultByClassId(classId);
    //
    //         var result = attendanceResults.ToDictionary(
    //             ar => ar.StudentId,
    //             ar => ar.IsPassed
    //         );
    //
    //         foreach (var studentClass in studentClasses)
    //         {
    //             result.TryAdd(studentClass.StudentFirebaseId, false);
    //         }
    //
    //         return result;
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Error checking attendance thresholds: {ex.Message}");
    //         return studentClasses.ToDictionary(
    //             sc => sc.StudentFirebaseId,
    //             _ => false
    //         );
    //     }
    // }

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

        // var attendanceResults = await ValidStudentsAttendance(classId, studentClasses);

        // Prepare collections for updates
        var studentClassUpdates = new List<StudentClass>();
        var studentUpdates = new List<Account>();
        var passedStudents = new List<StudentClass>();
        var studentAttendanceResults = await _serviceFactory.SlotService.GetAllAttendanceResultByClassId(classId);
        
        
        decimal minimumGpa = classInfo.Level?.MinimumGPA ?? DefaultPassingGrade;
        foreach (var studentClass in studentClasses)
        {
            var student = studentAccountMap[studentClass.StudentFirebaseId];

            // Get student's attendance status
            // bool meetsAttendanceThreshold =
            //     attendanceResults.TryGetValue(studentClass.StudentFirebaseId, out bool attendanceStatus)
            //         ? attendanceStatus
            //         : false;

            var studentAttendanceResult =
                studentAttendanceResults.FirstOrDefault(x => x.StudentId == student.AccountFirebaseId);
            if (studentAttendanceResult == null)
            {
                _logger.LogWarning("No attendance data found for student {StudentId} in class {ClassId}", 
                    student.AccountFirebaseId, classId);
            }

            var isPassed = false;

            if (studentAttendanceResult != null && 
                studentAttendanceResult.IsPassed && 
                studentClass.GPA.HasValue && 
                studentClass.GPA.Value >= minimumGpa)
            {
                studentClass.IsPassed = true;
                studentClass.UpdateById = account.AccountFirebaseId;
                studentClass.UpdatedAt = updateTime;
                studentClassUpdates.Add(studentClass);
            }

            passedStudents.Add(studentClass);

            // Update student level if passed
            if (studentClass.IsPassed && classInfo.Level?.NextLevelId.HasValue == true)
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

    private async Task SendClassCompletionNotifications(List<StudentClass> studentClasses, Class classInfo)
    {
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
                "Scores have been published",
                string.Format(InstructorNotificationTemplate, classInfo.Name)
            ));
        }

        int batchSize = 5;
        for (int i = 0; i < notifications.Count; i += batchSize)
        {
            var batch = notifications.Skip(i).Take(batchSize).ToList();
            foreach (var (recipientId, title, content) in batch)
            {
                await _serviceFactory.NotificationService.SendNotificationAsync(recipientId, title, content);
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
                    criteriaScores = criteriaScores.OrderBy(cs => DetermineCriteriaOrder(cs.CriteriaName)).ToList();
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

    public async Task<StudentDetailedScoreViewModel> GetStudentDetailedScores(Guid studentClassId)
    {
        // Validate input
        if (studentClassId == Guid.Empty)
        {
            throw new ArgumentException("Invalid student class ID", nameof(studentClassId));
        }

        // Get student class information
        var studentClass = await _unitOfWork.StudentClassRepository.GetByIdAsync(studentClassId);
        if (studentClass == null)
        {
            throw new NotFoundException($"Student class with ID {studentClassId} not found");
        }

        // Get class information
        var classInfo = await _unitOfWork.ClassRepository.GetByIdAsync(studentClass.ClassId);
        if (classInfo == null)
        {
            throw new NotFoundException($"Class with ID {studentClass.ClassId} not found");
        }

        // Get student information
        var student = await _unitOfWork.AccountRepository.FindFirstAsync(a =>
            a.AccountFirebaseId == studentClass.StudentFirebaseId);
        if (student == null)
        {
            throw new NotFoundException($"Student with ID {studentClass.StudentFirebaseId} not found");
        }

        // Get all scores for this student in the class
        var scores =
            await _unitOfWork.StudentClassScoreRepository.FindAsync(scs => scs.StudentClassId == studentClassId);

        // Get all criteria IDs from the scores
        var criteriaIds = scores.Select(s => s.CriteriaId).Distinct().ToList();

        // Get all criteria details
        var criteria = await _unitOfWork.CriteriaRepository.FindAsync(c => criteriaIds.Contains(c.Id));
        var criteriaDict = criteria.ToDictionary(c => c.Id);

        // Create detailed score view models
        var detailedScores = new List<CriteriaScoreViewModel>();
        foreach (var score in scores)
        {
            if (criteriaDict.TryGetValue(score.CriteriaId, out var criteriaInfo))
            {
                detailedScores.Add(new CriteriaScoreViewModel
                {
                    CriteriaId = score.CriteriaId,
                    CriteriaName = criteriaInfo.Name,
                    Weight = criteriaInfo.Weight,
                    Score = score.Score
                });
            }
            else
            {
                // Handle missing criteria information
                detailedScores.Add(new CriteriaScoreViewModel
                {
                    CriteriaId = score.CriteriaId,
                    CriteriaName = "Unknown Criteria",
                    Weight = 0,
                    Score = score.Score
                });
            }
        }

        // Sort criteria scores by weight for consistency
        var sortedScores = detailedScores.OrderBy(cs => DetermineCriteriaOrder(cs.CriteriaName)).ToList();

        // Create and return the view model
        return new StudentDetailedScoreViewModel
        {
            StudentId = studentClass.StudentFirebaseId,
            StudentName = student.FullName ?? student.UserName ?? "Unknown Student",
            StudentClassId = studentClassId,
            ClassId = studentClass.ClassId,
            ClassName = classInfo.Name,
            Gpa = studentClass.GPA,
            IsPassed = studentClass.IsPassed,
            CriteriaScores = sortedScores,
            InstructorComment = studentClass.InstructorComment,
            CertificateUrl = studentClass.CertificateUrl
        };
    }

    private int DetermineCriteriaOrder(string criteriaName)
    {
        if (string.IsNullOrEmpty(criteriaName))
            return 1000; // Default for empty names

        // Define category priorities
        var categoryPriorities = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            // Test categories
            { "Test", 100 },
            { "Stability Test", 110 },
            { "Coordination Test", 120 },

            // Assignment categories
            { "Assignment", 200 },
            { "Practice Assignment", 210 },

            // Workshop categories
            { "Workshop", 300 },
            { "Techniques Workshop", 310 },

            // Training categories
            { "Training", 400 },
            { "Stretch Training", 410 },

            // Performance categories
            { "Performance", 500 },
            { "Expression Performance", 510 },

            // Project categories
            { "Project", 600 },
            { "Duet Project", 610 },

            // Specific skills
            { "Memorization", 700 },

            // Keep the existing Vietnamese categories for backward compatibility
            { "Kiểm tra nhỏ", 800 },
            { "Bài thi", 810 },
            { "Thi cuối kỳ", 820 },
            { "Điểm chuyên cần", 900 },

            // Catch-all (lowest priority)
            { "Others", 1000 }
        };

        // Subcategories for more precise sorting
        var subcategoryPriorities = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            // Technique types
            { "Tone", 10 },
            { "Rhythmic", 20 },
            { "Articulation", 30 },
            { "Expression", 40 },
            { "Arpeggios", 50 },
            { "Hand", 60 },
            { "Pedal", 70 },
            { "Duet", 80 },

            // Vietnamese subcategories for backward compatibility
            { "Âm sắc", 15 },
            { "Độ chính xác", 25 },
            { "Phong thái", 35 },
            { "Nhịp điệu", 45 }
        };

        // Find main category
        int baseOrder = 1000; // Default order
        foreach (var category in categoryPriorities)
        {
            if (criteriaName.IndexOf(category.Key, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                baseOrder = category.Value;
                break;
            }
        }

        // Find subcategory modifier
        int subOrder = 0;
        foreach (var subcategory in subcategoryPriorities)
        {
            if (criteriaName.IndexOf(subcategory.Key, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                subOrder = subcategory.Value;
                break;
            }
        }

        // Check for numeric identifiers (like "Test 1", "Assignment 2", etc.)
        var numberMatch = System.Text.RegularExpressions.Regex.Match(criteriaName, @"\d+");
        if (numberMatch.Success)
        {
            int number = int.Parse(numberMatch.Value);
            // Add a small value for numeric ordering within the same category
            subOrder += number;
        }

        // Combine for final sort order
        return baseOrder + subOrder;
    }
    
    public async Task RollbackPublishScores(Guid classId, AccountModel account)
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
                throw new NotFoundException("Class not found");
            }
            
            if (!classInfo.IsScorePublished)
            {
                throw new BadRequestException("Cannot rollback: The class scores have not been published yet.");
            }

            if (classInfo.Level == null && classInfo.LevelId != Guid.Empty)
            {
                classInfo.Level = await _unitOfWork.LevelRepository.FindSingleAsync(l => l.Id == classInfo.LevelId);
                if (classInfo.Level == null)
                {
                    throw new NotFoundException("Level not found");
                }
            }
            var studentClasses = await _unitOfWork.StudentClassRepository.FindAsync(sc => sc.ClassId == classId);
            if (!studentClasses.Any())
            {
                throw new NotFoundException("No students enrolled in this class.");
            }
            
            // Get all student accounts
            var studentFirebaseIds = studentClasses.Select(sc => sc.StudentFirebaseId).Distinct().ToList();
            var studentAccounts = await _unitOfWork.AccountRepository
                .FindAsync(a => studentFirebaseIds.Contains(a.AccountFirebaseId));
        
            classInfo.IsScorePublished = false;
            classInfo.UpdateById = account.AccountFirebaseId;
            classInfo.UpdatedAt = DateTime.UtcNow.AddHours(7);
            
            var studentClassUpdates = new List<StudentClass>();
            var studentAccountUpdates = new List<Account>();

            foreach (var studentClass in studentClasses)
            {
                studentClass.IsPassed = false;
                studentClass.CertificateUrl = null;       
                studentClass.CertificateHtml = null;      
                studentClass.HasCertificateHtml = false;  
                studentClass.UpdateById = account.AccountFirebaseId;
                studentClass.UpdatedAt = DateTime.UtcNow.AddHours(7);
                studentClassUpdates.Add(studentClass);
                
                var student = studentAccounts.FirstOrDefault(a => a.AccountFirebaseId == studentClass.StudentFirebaseId);
                if (student != null)
                {
                    // Only reset level for students who passed and were advanced to next level
                    if (studentClass.IsPassed == true && classInfo.Level?.NextLevelId.HasValue == true)
                    {
                        // If the student is at the next level, reset them back to the class level
                        if (student.LevelId == classInfo.Level.NextLevelId.Value)
                        {
                            student.LevelId = classInfo.LevelId;
                        }
                    }
                
                    // Reset status back to InClass for all students
                    student.StudentStatus = StudentStatus.InClass;
                    student.UpdatedAt = DateTime.UtcNow.AddHours(7);
                    studentAccountUpdates.Add(student);
                }
            }
            
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Update class first
                await _unitOfWork.ClassRepository.UpdateAsync(classInfo);

                // Update student classes and accounts
                await _unitOfWork.StudentClassRepository.UpdateRangeAsync(studentClassUpdates);
                await _unitOfWork.AccountRepository.UpdateRangeAsync(studentAccountUpdates);
            });

            await SendScoreRollbackNotifications(studentClasses, classInfo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error rolling back scores for class {classId}: {ex.Message}");
            throw;
        }
    }
    
    private async Task SendScoreRollbackNotifications(List<StudentClass> studentClasses, Class classInfo)
    {
        const string RollbackNotificationTemplate = 
            "The music center has identified some issues with the scoring process for class {0}. As a result, the published scores have been temporarily rolled back while we resolve these concerns. We apologize for any inconvenience and will notify you once the corrected scores are available.";

        var notifications = new List<(string recipientId, string title, string content)>();

        foreach (var studentClass in studentClasses)
        {
            var title = "Important Notice: Class Score Update";
            var content = string.Format(RollbackNotificationTemplate, classInfo.Name);
            notifications.Add((studentClass.StudentFirebaseId, title, content));
        }

        // Add instructor notification if an instructor exists
        if (!string.IsNullOrEmpty(classInfo.InstructorId))
        {
            notifications.Add((
                classInfo.InstructorId,
                "Score Publishing Rollback Notice",
                $"The published scores for class {classInfo.Name} have been rolled back due to scoring concerns."
            ));
        }

        int batchSize = 5;
        for (int i = 0; i < notifications.Count; i += batchSize)
        {
            var batch = notifications.Skip(i).Take(batchSize).ToList();
            foreach (var (recipientId, title, content) in batch)
            {
                await _serviceFactory.NotificationService.SendNotificationAsync(recipientId, title, content);
            }
        }
    }
}