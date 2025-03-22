using System.ComponentModel.DataAnnotations;
using PhotonPiano.Api.Requests.SurveyQuestion;

namespace PhotonPiano.Api.Requests.Survey;

public record CreatePianoSurveyRequest : IValidatableObject
{
    [Required(ErrorMessage = "Name is required")]
    public required string Name { get; init; }

    public string? Description { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Min age must >= 1")]
    public int? MinAge { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Max age must >= 1")]
    public int? MaxAge { get; init; }

    public HashSet<CreateQuestionInSurveyRequest> Questions { get; init; } = [];
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!Name.Contains("piano", StringComparison.CurrentCultureIgnoreCase))
        {
            yield return new ValidationResult("Survey name must contains piano keyword");
        }

        if (MinAge >= MaxAge)
        {
            yield return new ValidationResult("Max age must be greater than Min age");
        }
    }
}