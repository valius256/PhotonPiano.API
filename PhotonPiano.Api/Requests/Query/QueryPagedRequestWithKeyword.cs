using Microsoft.AspNetCore.Mvc;

namespace PhotonPiano.Api.Requests.Query;

public record QueryPagedRequestWithKeyword : QueryPagedRequest
{
    [FromQuery(Name = "q")] 
    public string? Keyword { get; init; }
}