using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.SurveyQuestion;

public record SurveyQuestionModel
{
    public required Guid Id { get; init; }
    
    public QuestionType Type { get; init; }
    
    public string QuestionContent { get; init; } = default!;
    
    public List<string> Options { get; init; } = default!;

    public int OrderIndex { get; init; }
    
    public bool AllowOtherAnswer { get; init; }
    
    public bool IsRequired { get; init; }
    
    public required string CreatedById { get; init; }
    
    public string? UpdatedById { get; init; }
}