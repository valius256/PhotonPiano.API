namespace PhotonPiano.BusinessLogic.BusinessModel.SurveyQuestion;

public record UpdateSurveyQuestionModel
{
    public string? QuestionContent { get; init; }

    public List<string>? Options { get; init; }
    
    public bool? AllowMultipleAnswers { get; init; }
    
    public bool? AllowOtherAnswer { get; init; }
}