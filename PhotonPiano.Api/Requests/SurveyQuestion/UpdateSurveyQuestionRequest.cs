namespace PhotonPiano.Api.Requests.SurveyQuestion;

public record UpdateSurveyQuestionRequest
{
    public string? QuestionContent { get; init; }

    public List<string>? Options { get; init; } 
}