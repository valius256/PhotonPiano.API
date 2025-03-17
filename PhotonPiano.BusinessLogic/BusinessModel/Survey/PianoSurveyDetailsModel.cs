using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.SurveyQuestion;

namespace PhotonPiano.BusinessLogic.BusinessModel.Survey;

public record PianoSurveyDetailsModel : PianoSurveyModel
{
    public AccountModel CreatedBy { get; init; } = default!;
    public ICollection<LearnerSurveyWithAnswersModel> LearnerSurveys { get; init; } = [];
    
    public ICollection<SurveyQuestionModel> Questions { get; init; } = [];
}