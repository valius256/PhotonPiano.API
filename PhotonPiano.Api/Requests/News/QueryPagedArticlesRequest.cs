using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Requests.Query;

namespace PhotonPiano.Api.Requests.News;

public record QueryPagedArticlesRequest : QueryPagedRequest
{
    [FromQuery(Name = "q")]
    public string? Keyword { get; init; }

    [FromQuery(Name = "published")]
    public bool? IsPublished { get; init; }
}