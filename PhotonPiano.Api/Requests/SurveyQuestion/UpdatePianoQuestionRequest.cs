namespace PhotonPiano.Api.Requests.SurveyQuestion;

public record UpdatePianoQuestionRequest
{
    public string? QuestionContent { get; init; }

    public List<string>? Options { get; init; } 
    
    public bool? AllowMultipleAnswers { get; init; }
    
    public bool? AllowOtherAnswer { get; init; }
}