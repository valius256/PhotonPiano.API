
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.FreeSlot
{
    public record FreeSlotModel
    {
        public Guid Id { get; init; }
        public DayOfWeek DayOfWeek { get; init; }
        public Shift Shift { get; init; }
        public string AccountId { get; init; } = string.Empty;
        public Guid? LevelId { get; init; }
    }
}
