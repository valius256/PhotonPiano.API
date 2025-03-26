namespace PhotonPiano.BusinessLogic.BusinessModel.SurveyQuestion;

public record CreateSurveyQuestionModel
{
    public required string QuestionContent { get; init; }

    public required List<string> Options { get; init; }
}