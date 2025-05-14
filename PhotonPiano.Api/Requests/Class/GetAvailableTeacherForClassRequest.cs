using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Requests.Query;

namespace PhotonPiano.Api.Requests.Class;

public record GetAvailableTeacherForClassRequest : QueryPagedRequest
{
    [FromQuery(Name = "class-id")] public Guid ClassId { get; init; }
    [FromQuery(Name = "keyword")] public string? Keyword { get; init; }
}