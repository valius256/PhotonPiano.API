using System.ComponentModel.DataAnnotations;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.SurveyQuestion;

public record CreateSurveyQuestionRequest
{
    [Required] 
    public required QuestionType Type { get; init; }
    
    [Required]
    public required string QuestionContent { get; init; }

    public List<string> Options { get; init; } = [];
    
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Order index must be >= 0")]
    public int OrderIndex { get; init; }

    public Guid SurveyId { get; init; }
    
    public bool AllowOtherAnswer { get; init; } = true;

    public bool IsRequired { get; init; } = true;
}