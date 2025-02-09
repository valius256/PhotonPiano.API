using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.SlotStudent;

public record SlotStudentModel
{
    public required Guid SlotId { get; set; }
    public required string StudentFirebaseId { get; set; }

    public AttendanceStatus AttendanceStatus { get; set; }

    public AccountModel StudentAccount { get; set; } = default!;
}