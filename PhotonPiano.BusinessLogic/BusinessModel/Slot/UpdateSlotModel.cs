
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Enums;

namespace PhotonPiano.BusinessLogic.BusinessModel.Slot
{
    public record UpdateSlotModel
    {
        public Guid Id { get; init; }
        public Shift? Shift { get; init; }
        public DateOnly? Date { get; init; }
        public Guid? RoomId { get; init; }
        public string? TeacherId { get; init; }
        public string? Reason { get; init; }
    }
}
