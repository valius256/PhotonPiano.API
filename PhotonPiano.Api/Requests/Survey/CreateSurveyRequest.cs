namespace PhotonPiano.Api.Requests.Survey;

public record CreateSurveyRequest
{
    public required Guid SurveyQuestionId { get; set; }
    public string QuestionContent { get; set; } = default!;
    public List<string> Answers { get; set; } = default!;
    public List<string> Options { get; set; } = default!;
    public bool AllowMultipleAnswers { get; set; }
}