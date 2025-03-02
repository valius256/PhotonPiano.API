
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Class
{
    public record ScheduleClassModel
    {
        public required Guid Id { get; init; }

        public List<DayOfWeek> DayOfWeeks { get; init; } = [];

        public Shift Shift { get; init; }

        public Guid? RoomId { get; init; }
    }
}
