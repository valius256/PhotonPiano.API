using PhotonPiano.BusinessLogic.BusinessModel.Class;

namespace PhotonPiano.BusinessLogic.BusinessModel.Slot;

public record SlotWithClassModel : SlotModel
{
    public ClassModel Class { get; init; } = default!;
}