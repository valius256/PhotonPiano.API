using PhotonPiano.BusinessLogic.BusinessModel.Account;

namespace PhotonPiano.BusinessLogic.BusinessModel.Survey;

public record PianoSurveyDetailsModel : PianoSurveyModel
{
    public AccountModel CreatedBy { get; init; } = default!;
    public ICollection<LearnerSurveyWithAnswersModel> LearnerSurveys { get; init; } = [];
    
    public ICollection<PianoSurveyQuestionWithQuestionModel> PianoSurveyQuestions { get; init; } = [];
}