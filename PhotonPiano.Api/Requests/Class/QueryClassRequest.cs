using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Requests.Query;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.Class
{
    public record QueryClassRequest : QueryPagedRequest
    {
        [FromQuery(Name = "statuses")] public List<ClassStatus> ClassStatus { get; init; } = [];

        [FromQuery(Name = "levels")] public List<Guid> Levels { get; init; } = [];

        [FromQuery(Name = "keyword")] public string? Keyword { get; init; }

        [FromQuery(Name = "is-score-published")] public bool? IsScorePublished { get; init; }

        [FromQuery(Name = "is-public")] public bool? IsPublic { get; init; }

        [FromQuery(Name = "teacher-id")] public string? TeacherId { get; init; }

        [FromQuery(Name = "student-id")] public string? StudentId { get; init; }
    }
}
