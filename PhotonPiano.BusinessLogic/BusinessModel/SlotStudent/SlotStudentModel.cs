using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.SlotStudent;

public record SlotStudentModel
{
    public required Guid SlotId { get; init; }
    public required string StudentFirebaseId { get; init; }

    public AttendanceStatus AttendanceStatus { get; init; }

    public AccountModel StudentAccount { get; init; } = default!;

    public string? AttendanceComment { get; init; }

    public string? GestureComment { get; init; }

    public string? GestureUrl { get; init; }

    public string? FingerNoteComment { get; init; }

    public string? PedalComment { get; init; }
}