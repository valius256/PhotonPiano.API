namespace PhotonPiano.BusinessLogic.BusinessModel.Survey;

public record LearnerAnswerModel
{
    public required Guid Type { get; init; }
    
    public Guid LearnerSurveyId { get; init; }

    public Guid SurveyQuestionId { get; init; }

    public List<string> Answers { get; init; } = [];
}