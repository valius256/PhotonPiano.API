using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.EntranceTest;

public record UpdateEntranceTestSystemConfigRequest : IValidatableObject
{
    [Range(1, int.MaxValue, ErrorMessage = "Min students must > 0.")]
    public int? MinStudentsPerEntranceTest { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Max students must > 0.")]
    public int? MaxStudentsPerEntranceTest { get; init; }

    public bool? AllowEntranceTestRegistering { get; init; }

    public int? TestFee { get; init; }

    public decimal? TheoryPercentage { get; init; }
    
    public decimal? PracticePercentage { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MinStudentsPerEntranceTest > MaxStudentsPerEntranceTest)
        {
            yield return new ValidationResult("Min student must < max students",
                [nameof(MinStudentsPerEntranceTest), nameof(MaxStudentsPerEntranceTest)]);
        }
    }
}