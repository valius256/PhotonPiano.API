using PhotonPiano.BusinessLogic.BusinessModel.Account;

namespace PhotonPiano.BusinessLogic.BusinessModel.Survey;

public record LearnerSurveyDetailsModel : LearnerSurveyModel
{
    public AccountModel Account { get; init; } = default!;
}