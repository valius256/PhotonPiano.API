using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Survey;

namespace PhotonPiano.BusinessLogic.BusinessModel.SurveyQuestion;

public record SurveyQuestionDetailsModel : SurveyQuestionModel
{
    public AccountModel CreatedBy { get; init; } = default!;
    
    public AccountModel UpdatedBy { get; init; } = default!;

    public ICollection<PianoSurveyModel> PianoSurveys { get; set; } = [];
    
    public ICollection<LearnerAnswerModel> LearnerAnswers { get; set; } = [];
}