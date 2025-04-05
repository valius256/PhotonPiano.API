namespace PhotonPiano.BusinessLogic.BusinessModel.Survey;

public record CreateLearnerAnswerModel
{
    public required Guid SurveyQuestionId { get; init; }
    
    public required List<string> Answers { get; init; }
}