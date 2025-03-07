
using PhotonPiano.BusinessLogic.BusinessModel.Query;

namespace PhotonPiano.BusinessLogic.BusinessModel.DayOff
{
    public record QueryDayOffModel : QueryPagedModel
    {
        public string? Name { get; init; }
        public DateTime? StartTime { get; init; }
        public DateTime? EndTime { get; init; }

        public string GetLikeKeyword()
        {
            return string.IsNullOrEmpty(Name) ? string.Empty : $"%{Name}%";
        }

    }
}
