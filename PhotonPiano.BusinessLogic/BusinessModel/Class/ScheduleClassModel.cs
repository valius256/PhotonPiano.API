
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Enums;

namespace PhotonPiano.BusinessLogic.BusinessModel.Class
{
    public record ScheduleClassModel
    {
        public required Guid Id { get; init; }

        public DateOnly StartWeek { get; init; }

        public List<DayOfWeek> DayOfWeeks { get; init; } = [];

        public Shift Shift { get; init; }

        public bool IsValidDayOfWeeks()
        {
            return DayOfWeeks.All(d => Enum.IsDefined(typeof(DayOfWeek), d));
        }
    }
}
