
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Slot
{
    public record UpdateSlotModel
    {
        public required Guid Id { get; set; }
        public required Shift? Shift { get; init; }
        public required DateOnly? Date { get; init; }
        public required Guid? RoomId { get; init; }
        public string? Reason { get; init; }
    }
}
