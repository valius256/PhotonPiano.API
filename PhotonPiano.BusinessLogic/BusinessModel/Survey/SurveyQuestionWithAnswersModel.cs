using PhotonPiano.BusinessLogic.BusinessModel.SurveyQuestion;

namespace PhotonPiano.BusinessLogic.BusinessModel.Survey;

public record SurveyQuestionWithAnswersModel : SurveyQuestionModel
{
    public ICollection<LearnerAnswerModel> LearnerAnswers { get; init; } = [];
}