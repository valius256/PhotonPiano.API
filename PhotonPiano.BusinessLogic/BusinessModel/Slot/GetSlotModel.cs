using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Slot;

public record GetSlotModel
{
    public required DateOnly StartTime { get; init; }
    public required DateOnly EndTime { get; init; }
    public List<Shift>? Shifts { get; set; }
    public List<SlotStatus>? SlotStatuses { get; init; }
    public string? InstructorFirebaseId { get; init; }
    public string? StudentFirebaseId { get; init; }
}