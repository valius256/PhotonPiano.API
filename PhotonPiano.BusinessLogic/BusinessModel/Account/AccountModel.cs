using PhotonPiano.BusinessLogic.BusinessModel.Level;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Account;

public record AccountModel
{
    public required string AccountFirebaseId { get; init; }
    public string? UserName { get; init; }

    public string? Phone { get; init; } = string.Empty;

    public string? FullName { get; init; }

    public required string Email { get; init; }
    public Role Role { get; init; }

    public string? AvatarUrl { get; init; } = string.Empty;

    public DateTime? DateOfBirth { get; init; }

    public string? Address { get; init; } = string.Empty;

    public Gender? Gender { get; init; }

    public string? BankAccount { get; init; } = string.Empty;

    public bool IsEmailVerified { get; init; } = false;

    public DateTime JoinedDate { get; init; } = DateTime.UtcNow.AddHours(7);

    public string? ShortDescription { get; init; } = string.Empty;

    public Guid? LevelId { get; init; }

    public AccountStatus Status { get; init; }

    public DateTime? RegistrationDate { get; init; }
    public StudentStatus? StudentStatus { get; init; }

    public string? DesiredLevel { get; init; }

    public List<string> DesiredTargets { get; init; } = [];

    public List<string> FavoriteMusicGenres { get; init; } = [];

    public List<string> PreferredLearningMethods { get; init; } = [];

    public RecordStatus RecordStatus { get; init; }
    public Guid? CurrentClassId { get; init; }
    
    public LevelModel? Level { get; init; }
}