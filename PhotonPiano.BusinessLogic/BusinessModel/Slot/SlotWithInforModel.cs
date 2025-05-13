using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Enums;

namespace PhotonPiano.BusinessLogic.BusinessModel.Slot;

public record SlotWithInforModel
{
    public Guid Id { get; init; }
    public Guid ClassId { get; init; }
    public Guid? RoomId { get; init; }
    public Shift Shift { get; init; }
    public DateOnly Date { get; init; }
    public SlotStatus Status { get; init; }
}