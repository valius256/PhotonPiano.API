using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.Room;

namespace PhotonPiano.BusinessLogic.BusinessModel.Slot;

public record SlotDetailModel : SlotModel
{
    public RoomModel Room { get; init; } = default!;
    public ClassModel Class { get; init; } = default!;
}