using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Requests.Query;

namespace PhotonPiano.Api.Requests.DayOff
{
    public record QueryDayOffRequest : QueryPagedRequest
    {
        [FromQuery(Name = "name")] public string? Name { get; init; }
        [FromQuery(Name = "start-time")] public DateTime? StartTime { get; init; }
        [FromQuery(Name = "end-time")] public DateTime? EndTime { get; init; }
    }
}
