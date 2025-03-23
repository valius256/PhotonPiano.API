using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Slot;

public record SlotModel
{
    public Guid Id { get; init; }
    public Guid? ClassId { get; init; }
    public Guid? RoomId { get; init; }
    public Shift Shift { get; init; }
    public DateOnly Date { get; init; }
    public SlotStatus Status { get; init; }
    public string? SlotNote { get; init; }
}