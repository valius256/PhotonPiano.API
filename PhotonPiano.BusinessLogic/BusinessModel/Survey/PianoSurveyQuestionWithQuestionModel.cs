using PhotonPiano.BusinessLogic.BusinessModel.SurveyQuestion;

namespace PhotonPiano.BusinessLogic.BusinessModel.Survey;

public record PianoSurveyQuestionWithQuestionModel : PianoSurveyQuestionModel
{
    public SurveyQuestionModel Question { get; init; } = default!;
}