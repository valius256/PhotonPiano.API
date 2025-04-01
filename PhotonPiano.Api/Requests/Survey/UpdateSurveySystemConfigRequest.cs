using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.Survey;

public record UpdateSurveySystemConfigRequest : IValidatableObject
{
    public string? InstrumentName { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Frequency must > 0")]
    public int? InstrumentFrequencyInResponse { get; init; }

    public string? EntranceSurveyId { get; init; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Max questions must > 0")]
    public int? MaxQuestionsPerSurvey { get; init; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Min questions must > 0")]
    public int? MinQuestionsPerSurvey { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MinQuestionsPerSurvey.HasValue && MaxQuestionsPerSurvey.HasValue &&
            MinQuestionsPerSurvey >= MaxQuestionsPerSurvey)
        {
            yield return new ValidationResult("Min num of questions must smaller than Max questions per survey",
                [nameof(MinQuestionsPerSurvey), nameof(MaxQuestionsPerSurvey)]);
        }
    }
}