using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Survey;

namespace PhotonPiano.BusinessLogic.BusinessModel.PianoQuestion;

public record PianoQuestionDetailsModel : PianoQuestionModel
{
    public AccountModel CreatedBy { get; init; } = default!;
    
    public AccountModel UpdatedBy { get; init; } = default!;
    
    public ICollection<LearnerSurveyDetailsModel> LearnerSurveys { get; init; } = default!;
}