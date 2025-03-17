using PhotonPiano.BusinessLogic.BusinessModel.SurveyQuestion;

namespace PhotonPiano.BusinessLogic.BusinessModel.Survey;

public record PianoSurveyWithQuestionsModel : PianoSurveyModel
{
    public ICollection<SurveyQuestionModel> SurveyQuestions { get; init; } = [];
}