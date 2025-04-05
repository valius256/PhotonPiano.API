namespace PhotonPiano.BusinessLogic.BusinessModel.Survey;

public record LearnerAnswerWithLearnerModel : LearnerAnswerModel
{
    public LearnerSurveyWithSurveyModel LearnerSurvey { get; init; } = default!;
}