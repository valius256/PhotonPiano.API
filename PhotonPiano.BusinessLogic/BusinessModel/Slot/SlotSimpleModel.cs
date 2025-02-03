using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.Slot;

public record SlotSimpleModel : SlotModel
{
    public RoomWithNameModel Room { get; init; } = default!;
    public ClassSimpleModel Class { get; init; } = default!;
}