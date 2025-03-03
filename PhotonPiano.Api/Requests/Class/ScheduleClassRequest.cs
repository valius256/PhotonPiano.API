using PhotonPiano.DataAccess.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.Class
{
    public record ScheduleClassRequest
    {
        public required Guid Id { get; init; }

        public DateOnly StartWeek { get; init; }

        public List<DayOfWeek> DayOfWeeks { get; init; } = [];

        public Shift Shift { get; init; }

        
    }
}
