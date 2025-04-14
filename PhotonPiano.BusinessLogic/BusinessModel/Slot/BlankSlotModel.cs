using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Enums;

namespace PhotonPiano.BusinessLogic.BusinessModel.Slot;

public record BlankSlotModel
{
    public DateOnly Date { get; init; }
    public Shift Shift { get; init; }
    public Guid RoomId { get; init; }
    public string? RoomName { get; init; }
}