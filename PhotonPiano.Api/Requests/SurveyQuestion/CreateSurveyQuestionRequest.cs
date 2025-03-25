using System.ComponentModel.DataAnnotations;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.SurveyQuestion;

public record CreateSurveyQuestionRequest : IValidatableObject
{
    public Guid? Id { get; init; }
    
    [Required] 
    public required QuestionType Type { get; init; }
    
    [Required]
    public required string QuestionContent { get; init; }

    public List<string> Options { get; init; } = [];
    
    [Range(0, int.MaxValue, ErrorMessage = "Order index must be >= 0")]
    public int? OrderIndex { get; init; }

    public Guid? SurveyId { get; init; }
    
    public bool IsRequired { get; init; } = true;
    
    public bool AllowOtherAnswer { get; init; } = true;
    
    [Range(1, int.MaxValue, ErrorMessage = "Min age must >= 1")]
    public int? MinAge { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Max age must >= 1")]
    public int? MaxAge { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MinAge >= MaxAge)
        {
            yield return new ValidationResult("Max age must be greater than Min age");
        }
    }
}