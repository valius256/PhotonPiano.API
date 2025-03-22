namespace PhotonPiano.BusinessLogic.BusinessModel.Slot;

public class CancelSlotModel
{
    public Guid SlotId { get; init; }
    public required string CancelReason { get; init; }
}