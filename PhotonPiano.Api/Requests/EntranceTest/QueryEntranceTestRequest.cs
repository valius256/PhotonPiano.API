using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Requests.Query;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Enums;

namespace PhotonPiano.Api.Requests.EntranceTest;

public record QueryEntranceTestRequest : QueryPagedRequest
{
    [FromQuery(Name = "room-ids")] public List<Guid>? RoomIds { get; init; } = [];

    [FromQuery(Name = "keyword")] public string? Keyword { get; init; }

    [FromQuery(Name = "shifts")] public List<Shift>? Shifts { get; init; } = [];

    [FromQuery(Name = "entrancetest-ids")] public List<Guid>? EntranceTestIds { get; init; } = [];

    [FromQuery(Name = "is-announced-score")]
    public bool? IsAnnouncedScore { get; init; }

    [FromQuery(Name = "instructor-ids")] public List<string>? InstructorIds { get; init; } = [];
}