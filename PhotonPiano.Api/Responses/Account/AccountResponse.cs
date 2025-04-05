using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Responses.Account;

public record AccountResponse
{
    public required string AccountFirebaseId { get; init; }
    public string? FullName { get; init; }
    public string? UserName { get; init; }
    public string? Phone { get; init; }
    public required string Email { get; init; }
    public Role Role { get; init; } = Role.Guest;
    public string? PictureUrl { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public string? Address { get; init; }
    public Gender? Gender { get; init; }
    public string? BankAccount { get; init; }

    public DateTime JoinedDate { get; init; }
    public string? ShortDescription { get; init; }
    public Guid? LevelId { get; init; }
    public AccountStatus Status { get; init; }
    public DateTime? RegistrationDate { get; init; }
    public RecordStatus RecordStatus { get; init; }
    public Guid? CurrentClassId { get; set; }
}