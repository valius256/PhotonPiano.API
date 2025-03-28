namespace PhotonPiano.BusinessLogic.BusinessModel.Survey;

public record LearnerSurveyWithAnswersModel : LearnerSurveyModel
{
    public ICollection<LearnerAnswerModel> LearnerAnswers { get; init; } = [];
}