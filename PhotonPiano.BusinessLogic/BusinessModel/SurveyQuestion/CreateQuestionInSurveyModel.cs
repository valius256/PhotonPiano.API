using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.SurveyQuestion;

public record CreateQuestionInSurveyModel
{
    public Guid? Id { get; init; }
    
    public required QuestionType Type { get; init; }
    
    public required string QuestionContent { get; init; }

    public List<string> Options { get; init; } = [];

    public bool IsRequired { get; init; }
    
    public bool AllowOtherAnswer { get; init; } = true;
}