using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Account;

public record AccountSimpleModel
{
    public required string AccountFirebaseId { get; init; }
    public string UserName { get; init; } = default!;
    public string? FullName { get; init; }
    public string Phone { get; init; } = default!;
    public string Email { get; init; } = default!;
    public Role Role { get; init; }
    public Gender Gender { get; init; }
    public DateTime JoinedDate { get; init; }
    public string? ShortDescription { get; init; }
    public LevelEnum Level { get; init; }
    public AccountStatus Status { get; init; }
    public DateTime RegistrationDate { get; init; }
}