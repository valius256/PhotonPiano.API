using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Survey;

public record SendEntranceSurveyAnswersModel
{
    public required string Email { get; init; }

    public required string Password { get; init; }

    public required string FullName { get; init; }

    public required string Phone { get; init; }

    public required Gender Gender { get; init; }
    
    public Guid? SelfEvaluatedLevelId { get; init; }

    public required List<CreateLearnerAnswerModel> SurveyAnswers { get; init; }

    public void Deconstruct(out string email, out string password, out string fullName, out string phone,
        out Gender gender, out List<CreateLearnerAnswerModel> answers, out Guid? selfEvaluatedLevelId)
    {
        email = Email;
        password = Password;
        fullName = FullName;
        phone = Phone;
        gender = Gender;
        answers = SurveyAnswers;
        selfEvaluatedLevelId = SelfEvaluatedLevelId;
    }
}