namespace PhotonPiano.BusinessLogic.BusinessModel.SurveyQuestion;

public record SurveyQuestionModel
{
    public required Guid Id { get; init; }
    
    public string QuestionContent { get; init; } = default!;
    
    public List<string> Options { get; init; } = default!;
    
    public required string CreatedById { get; init; }
    
    public string? UpdatedById { get; init; }
    
    public bool AllowMultipleAnswers { get; init; }
}