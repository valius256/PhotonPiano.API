using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Requests.Query;

namespace PhotonPiano.Api.Requests.Survey;

public record QueryPagedSurveysRequest : QueryPagedRequest
{
    [FromQuery(Name = "q")] 
    public string? Keyword { get; init; }
}