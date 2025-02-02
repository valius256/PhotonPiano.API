using System.ComponentModel.DataAnnotations;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.EntranceTest;

public record AutoArrangeEntranceTestsRequest : IValidatableObject
{
    public required List<string> StudentIds { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }

    public List<Shift> ShiftOptions { get; init; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate < DateTime.UtcNow)
        {
            yield return new ValidationResult("Start date must be in the future.", [nameof(StartDate)]);
        }

        if (EndDate < DateTime.UtcNow)
        {
            yield return new ValidationResult("End date must be in the future.", [nameof(EndDate)]);
        }

        if (StartDate > EndDate)
        {
            yield return new ValidationResult("Start date must be before end date", [
                nameof(StartDate), nameof(EndDate)
            ]);
        }
    }
}