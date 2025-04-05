using System.ComponentModel.DataAnnotations;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.SurveyQuestion;

public record UpdateSurveyQuestionInSurveyModel
{
    public Guid? Id { get; init; }
    
    [Required] 
    public required QuestionType Type { get; init; }
    
    [Required]
    public required string QuestionContent { get; init; }

    public List<string> Options { get; init; } = [];
    
    public bool IsRequired { get; init; }
    
    public bool AllowOtherAnswer { get; init; } = true;
}