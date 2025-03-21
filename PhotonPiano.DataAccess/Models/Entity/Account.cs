using System.ComponentModel.DataAnnotations;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.Models.Entity;

public class Account : BaseEntity
{
    [StringLength(30, ErrorMessage = "Account firebaseId cannot be longer than 30 characters.")]
    public required string AccountFirebaseId { get; set; }

    public string? UserName { get; set; }
    public string? Phone { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public required string Email { get; set; }

    public string? FullName { get; set; }
    public Role Role { get; set; } = Role.Guest;
    public string? AvatarUrl { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; } = string.Empty;
    public Gender? Gender { get; set; }
    public string? BankAccount { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; } = false;
    public DateTime JoinedDate { get; set; } = DateTime.UtcNow.AddHours(7);
    public string? ShortDescription { get; set; } = string.Empty;
    public Guid? LevelId { get; set; }
    public AccountStatus Status { get; set; } = AccountStatus.Active;
    public StudentStatus? StudentStatus { get; set; }
    public int? DesiredLevel { get; set; }
    public List<string> DesiredTargets { get; set; } = [];
    public List<string> FavoriteMusicGenres { get; set; } = [];

    public List<string> PreferredLearningMethods { get; set; } = [];

    public Guid? CurrentClassId { get; set; }

    public virtual Level? Level { get; set; }

    public virtual Class? CurrentClass { get; set; }


    // reference 
    // EntranceTest
    public virtual ICollection<EntranceTest> InstructorEntranceTests { get; set; } = new List<EntranceTest>();
    public virtual ICollection<EntranceTest> CreatedEntrancesTest { get; set; } = new List<EntranceTest>();
    public virtual ICollection<EntranceTest> UpdatedEntrancesTest { get; set; } = new List<EntranceTest>();
    public virtual ICollection<EntranceTest> DeletedEntrancesTest { get; set; } = new List<EntranceTest>();


    // EntranceTestStudent
    public virtual ICollection<EntranceTestStudent> EntranceTestStudents { get; set; } =
        new List<EntranceTestStudent>();

    public virtual ICollection<EntranceTestStudent> CreatedEntranceTestStudent { get; set; } =
        new List<EntranceTestStudent>();

    public virtual ICollection<EntranceTestStudent> UpdatedEntranceTestStudent { get; set; } =
        new List<EntranceTestStudent>();

    public virtual ICollection<EntranceTestStudent> DeletedEntranceTestStudent { get; set; } =
        new List<EntranceTestStudent>();

    // Room
    public virtual ICollection<Room> CreatedRoom { get; set; } = new List<Room>();
    public virtual ICollection<Room> UpdatedRoom { get; set; } = new List<Room>();
    public virtual ICollection<Room> DeletedRoom { get; set; } = new List<Room>();

    // EntranceTestResult
    public virtual ICollection<EntranceTestResult> CreatedEntranceTestResult { get; set; } =
        new List<EntranceTestResult>();

    public virtual ICollection<EntranceTestResult> UpdatedEntranceTestResult { get; set; } =
        new List<EntranceTestResult>();

    public virtual ICollection<EntranceTestResult> DeletedEntranceTestResult { get; set; } =
        new List<EntranceTestResult>();

    // Criteria
    public virtual ICollection<Criteria> CreatedCriteria { get; set; } = new List<Criteria>();
    public virtual ICollection<Criteria> UpdatedCriteria { get; set; } = new List<Criteria>();
    public virtual ICollection<Criteria> DeletedCriteria { get; set; } = new List<Criteria>();

    // Transaction 
    public virtual ICollection<Transaction> CreatedTransaction { get; set; } = new List<Transaction>();

    // Class
    public virtual ICollection<Class> InstructorClasses { get; set; } = new List<Class>();
    public virtual ICollection<Class> CreatedClasses { get; set; } = new List<Class>();
    public virtual ICollection<Class> UpdatedClasses { get; set; } = new List<Class>();
    public virtual ICollection<Class> DeletedClasses { get; set; } = new List<Class>();

    // SlotStudent
    public virtual ICollection<SlotStudent> SlotStudents { get; set; } = new List<SlotStudent>();
    public virtual ICollection<SlotStudent> CreatedSlotStudents { get; set; } = new List<SlotStudent>();
    public virtual ICollection<SlotStudent> UpdatedSlotStudents { get; set; } = new List<SlotStudent>();
    public virtual ICollection<SlotStudent> DeletedSlotStudents { get; set; } = new List<SlotStudent>();


    // Notification
    public virtual ICollection<Notification> ReceiverNotifications { get; set; } = new List<Notification>();

    // StudentClass 
    public virtual ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();
    public virtual ICollection<StudentClass> CreatedStudentClass { get; set; } = new List<StudentClass>();
    public virtual ICollection<StudentClass> UpdatedStudentClass { get; set; } = new List<StudentClass>();
    public virtual ICollection<StudentClass> DeletedStudentClass { get; set; } = new List<StudentClass>();

    // New
    public virtual ICollection<New> CreatedNews { get; set; } = new List<New>();
    public virtual ICollection<New> UpdatednNews { get; set; } = new List<New>();
    public virtual ICollection<New> DeletednNews { get; set; } = new List<New>();

    // DayOff
    public virtual ICollection<DayOff> CreatedDayOffs { get; set; } = new List<DayOff>();
    public virtual ICollection<DayOff> UpdatedDayOffs { get; set; } = new List<DayOff>();
    public virtual ICollection<DayOff> DeletedDayOffs { get; set; } = new List<DayOff>();

    // AccountNotification 
    public virtual ICollection<AccountNotification> AccountNotifications { get; set; } =
        new List<AccountNotification>();

    // Application
    public virtual ICollection<Application> CreatedApplications { get; set; } = new List<Application>();
    public virtual ICollection<Application> UpdatedApplications { get; set; } = new List<Application>();
    public virtual ICollection<Application> ApprovedApplications { get; set; } = new List<Application>();
    public virtual ICollection<Application> DeletedApplications { get; set; } = new List<Application>();
    
    // SurveyQuestion
    public virtual ICollection<SurveyQuestion> CreatedSurveyQuestions { get; set; } = new List<SurveyQuestion>();
    public virtual ICollection<SurveyQuestion> UpdatedSurveyQuestions { get; set; } = new List<SurveyQuestion>();
    
    
    // SurveyDetails
    public virtual ICollection<LearnerSurvey> LearnerSurveys { get; set; } = new List<LearnerSurvey>();
    
    // Slot 
    public virtual ICollection<Slot> UpdatedSlots { get; set; } = new List<Slot>();
    public virtual ICollection<Slot> CanceledSlots { get; set; } = new List<Slot>();
}