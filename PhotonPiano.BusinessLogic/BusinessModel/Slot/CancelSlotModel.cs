using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.BusinessLogic.BusinessModel.Slot;

public class CancelSlotModel
{
    public Guid SlotId { get; init; }
    [Required(ErrorMessage = "Cancel reason is required")]
    public required string CancelReason { get; init; }
}