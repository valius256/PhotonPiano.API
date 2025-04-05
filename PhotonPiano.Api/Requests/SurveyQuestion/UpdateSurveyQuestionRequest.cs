namespace PhotonPiano.Api.Requests.SurveyQuestion;

public record UpdateSurveyQuestionRequest
{
    public string? QuestionContent { get; init; }

    public List<string>? Options { get; init; } 
    
    public int? MinAge { get; init; }
    
    public int? MaxAge { get; init; }
}