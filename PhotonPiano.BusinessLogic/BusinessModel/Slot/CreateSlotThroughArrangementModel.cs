
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Enums;

namespace PhotonPiano.BusinessLogic.BusinessModel.Slot
{
    public record CreateSlotThroughArrangementModel
    {
        public required Shift Shift { get; init; }

        public required DateOnly Date { get; init; }

        public required Guid RoomId { get; init; }
    }
}
