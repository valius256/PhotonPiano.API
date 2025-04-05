using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.SurveyQuestion;

public record CreateSurveyQuestionModel
{
    public Guid? Id { get; init; }
    public required QuestionType Type { get; init; }
  
    public required string QuestionContent { get; init; }
    public List<string> Options { get; init; } = [];
    
    public int? OrderIndex { get; init; }
    public Guid? SurveyId { get; init; }

    public bool IsRequired { get; init; } = true;

    public bool AllowOtherAnswer { get; init; } = true;
    public int MinAge { get; init; }

    public int? MaxAge { get; init; }
}