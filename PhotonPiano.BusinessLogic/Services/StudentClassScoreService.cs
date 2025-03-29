using PhotonPiano.BusinessLogic.BusinessModel.Account;
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
       private const decimal DEFAULT_PASSING_GRADE = 5.0m;
       private const string VIETNAM_TIMEZONE = "+07:00";   
       
    public StudentClassScoreService(IUnitOfWork unitOfWork, IServiceFactory serviceFactory)
    {
        _unitOfWork = unitOfWork;
        _serviceFactory = serviceFactory; }
    
    private async Task<decimal> GetPassingGradeForLevel(string levelName)
    {
        try
        {
            // Extract level number from level name (assuming format like "Level 2" or "Level 2 (Something)")
            string levelNumberStr = levelName.Split(' ')[1].Split('(')[0].Trim();
            if (!int.TryParse(levelNumberStr, out int levelNumber))
            {
                Console.WriteLine($"Could not parse level number from {levelName}, using default passing grade");
                return DEFAULT_PASSING_GRADE;
            }

            // Get the passing grade from system config
            var configKey = $"{LEVEL_PASSING_GRADE_CONFIG_PREFIX}{levelNumber}";
            var config = await _serviceFactory.SystemConfigService.GetConfig(configKey);
            
            if (config == null || string.IsNullOrEmpty(config.ConfigValue))
            {
                Console.WriteLine($"No passing grade config found for {configKey}, using default");
                return DEFAULT_PASSING_GRADE;
            }

            if (decimal.TryParse(config.ConfigValue, out decimal passingGrade))
            {
                return passingGrade;
            }
            else
            {
                Console.WriteLine($"Could not parse passing grade value {config.ConfigValue}, using default");
                return DEFAULT_PASSING_GRADE;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error determining passing grade for level {levelName}: {ex.Message}");
            return DEFAULT_PASSING_GRADE;
        }
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

            if (classInfo.Level == null)
            {
                classInfo.Level = await _unitOfWork.LevelRepository.FindSingleAsync(l => l.Id == classInfo.LevelId);
            }
            
            decimal passingGrade = await GetPassingGradeForLevel(classInfo.Level.Name);
            
            Level nextLevel = null;
            if (classInfo.Level.NextLevel != null)
            {
                try
                {
                    nextLevel = await _unitOfWork.LevelRepository.GetByIdAsync(classInfo.LevelId);
                }
                catch (NotFoundException ex)
                {
                    Console.WriteLine($"Next level (ID: {classInfo.Level.NextLevelId.Value}) not found");
                }
            }
            
            var studentClasses = await _unitOfWork.StudentClassRepository.FindAsync(sc => sc.ClassId == classId);
            
            if (!studentClasses.Any())
            {
                throw new BadRequestException("No students enrolled in this class.");
            }
            
            var studentsWithoutGPA = studentClasses.Where(sc => !sc.GPA.HasValue).ToList();
            if (studentsWithoutGPA.Any())
            {
                Console.WriteLine($"{studentsWithoutGPA.Count} students in class {classId} have no GPA values");
            }
            
            var studentFirebaseIds = studentClasses.Select(sc => sc.StudentFirebaseId).ToList();
            var studentAccounts = await _unitOfWork.AccountRepository.FindAsync(
                a => studentFirebaseIds.Contains(a.AccountFirebaseId));
            
            // Create a dictionary for faster lookups
            var studentAccountMap = studentAccounts.ToDictionary(a => a.AccountFirebaseId);

            var passedStudentIds = new List<string>();
            var failedStudentIds = new List<string>();

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                classInfo.IsScorePublished = true;
                classInfo.Status = ClassStatus.Finished;
                classInfo.UpdateById = account.AccountFirebaseId;
                classInfo.UpdatedAt = DateTime.UtcNow.AddHours(7); 
                
                await _unitOfWork.ClassRepository.UpdateAsync(classInfo);

                foreach (var studentClass in studentClasses)
                {
                    if (!studentClass.GPA.HasValue)
                    {
                        Console.WriteLine($"Student {studentClass.StudentFirebaseId} in class has no GPA value, skipping");
                        continue;
                    }

                    if (!studentAccountMap.TryGetValue(studentClass.StudentFirebaseId, out var student))
                    {
                        Console.WriteLine($"Student with FirebaseId {studentClass.StudentFirebaseId} not found in database");
                        continue;
                    }

                    // Update common student properties
                    student.StudentStatus = StudentStatus.WaitingForClass;
                    student.CurrentClassId = null;
                    
                    //Check if Passed based on GPA and the level's passing grade
                    bool isPassed = studentClass.GPA.Value >= passingGrade;
                    studentClass.IsPassed = isPassed;
                    if (isPassed)
                    {
                        studentClass.CertificateUrl = null;

                        if (nextLevel != null)
                        {
                            student.LevelId = nextLevel.Id;
                        }
                        
                        passedStudentIds.Add(studentClass.StudentFirebaseId);
                    }
                    else
                    {
                        failedStudentIds.Add(studentClass.StudentFirebaseId);   
                    }
                    
                    await _unitOfWork.AccountRepository.UpdateAsync(student);
                    
                    studentClass.UpdateById = account.AccountFirebaseId;
                    studentClass.UpdatedAt = DateTime.UtcNow.AddHours(7);
                    await _unitOfWork.StudentClassRepository.UpdateAsync(studentClass);
                }
                //Send Notification 
                // try
                // {
                //     await _serviceFactory.NotificationService.SendNotificationAsync(classId, account);
                // }
                // catch (Exception ex)
                // {
                //     _logger.LogError(ex, "Error sending notifications, but proceeding with score publication");
                //     // Continue with transaction - don't throw to allow score publication to complete
                // }
            });
            Console.WriteLine($"Successfully published scores for class {classId}. Passed: {passedStudentIds.Count}, Failed: {failedStudentIds.Count}");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error publishing scores for class {classId}: {ex.Message}");
            throw;
        }
    }
}