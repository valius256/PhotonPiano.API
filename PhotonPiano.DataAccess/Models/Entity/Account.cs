using System.ComponentModel.DataAnnotations;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.Models.Entity;

public class Account : BaseEntity
{
    [StringLength(30, ErrorMessage = "Account firebaseId cannot be longer than 30 characters.")]
    public required string AccountFirebaseId { get; set; }

    public string? Username { get; set; }
    public string? Phone { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public required string Email { get; set; }

    public Role Role { get; set; } = Role.Guest;
    public string? AvatarUrl { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; } = string.Empty;
    public Gender? Gender { get; set; }
    public string? BankAccount { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; } = false;
    public DateTime JoinedDate { get; set; } = DateTime.UtcNow.AddHours(7);
    public string? ShortDescription { get; set; } = string.Empty;
    public Level Level { get; set; } = Level.Beginner;
    public AccountStatus Status { get; set; } = AccountStatus.Active;
    public DateTime? RegistrationDate { get; set; }
    public RecordStatus RecordStatus { get; set; } = RecordStatus.IsActive;
    public StudentStatus? StudentStatus { get; set; }


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
}