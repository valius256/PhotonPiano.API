using PhotonPiano.BusinessLogic.BusinessModel.Slot;

namespace PhotonPiano.BusinessLogic.BusinessModel.Class;

public record ClassWithSlotsModel : ClassModel
{
    public ICollection<SlotModel> Slots { get; init; } = [];
}