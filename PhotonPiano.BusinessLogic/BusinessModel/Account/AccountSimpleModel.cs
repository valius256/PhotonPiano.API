using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Account;

public record AccountSimpleModel
{
    public required string AccountFirebaseId { get; init; }
    public string Name { get; init; }
    public string Phone { get; init; }
    public string Email { get; init; }
    public Role Role { get; init; }
    public Gender Gender { get; init; }
    public DateTime JoinedDate { get; init; }
    public string ShortDescription { get; init; }
    public Level Level { get; init; }
    public AccountStatus Status { get; init; }
    public DateTime RegistrationDate { get; init; }
}