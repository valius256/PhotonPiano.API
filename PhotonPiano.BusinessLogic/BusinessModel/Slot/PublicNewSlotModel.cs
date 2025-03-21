using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Slot;

public class PublicNewSlotModel
{
    public Guid RoomId { get; init; }
    public DateOnly Date { get; init; }
    public Shift Shift { get; init; }
    public Guid ClassId { get; init; }
}