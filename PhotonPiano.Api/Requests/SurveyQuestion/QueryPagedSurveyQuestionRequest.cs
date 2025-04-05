using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Requests.Query;

namespace PhotonPiano.Api.Requests.SurveyQuestion;

public record QueryPagedSurveyQuestionRequest : QueryPagedRequest
{
    [FromQuery(Name = "q")]
    public string? Keyword { get; init; }
}