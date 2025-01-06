using System.ComponentModel.DataAnnotations;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.Models.Entity
{
    public class Account : BaseEntity
    {
        [StringLength(maximumLength: 30, ErrorMessage = "Account firebaseId cannot be longer than 30 characters.")]
        public required string AccountFirebaseId { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; } = String.Empty;
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public required string Email { get; set; } 
        public Role Role { get; set; } = Role.Guest;
        public string? PictureUrl { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; } = string.Empty;
        public Gender? Gender { get; set; } 
        public string? BankAccount { get; set; } = string.Empty;

        public DateTime JoinedDate { get; set; } = DateTime.UtcNow.AddHours(7);
        public string? ShortDescription { get; set; } = string.Empty;
        public Level Level { get; set; } = Level.Beginner;
        public AccountStatus Status { get; set; } = AccountStatus.Active;
        public DateTime? RegistrationDate { get; set; }
        public RecordStatus RecordStatus { get; set; } = RecordStatus.IsActive;
        
        // reference 
        public virtual ICollection<EntranceTest> EntranceTests { get; set; } = new List<EntranceTest>();
        public virtual ICollection<EntranceTestStudent> EntranceTestStudents { get; set; } = new List<EntranceTestStudent>();
        
        
    }
}
