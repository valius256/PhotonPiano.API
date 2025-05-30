using System.ComponentModel.DataAnnotations;
using PhotonPiano.Shared.Enums;

namespace PhotonPiano.Api.Requests.EntranceTest;

public record UpdateEntranceTestRequest : IValidatableObject
{
    public string? Name { get; init; }
    
    public Guid? RoomId { get; init; }

    public Shift? Shift { get; init; }
    public DateOnly? Date { get; init; }

    public string? InstructorId { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Date.HasValue && Date.Value <= DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7)))
        {
            yield return new ValidationResult("Test date must be in the future.", [nameof(Date)]);
        }
    }
}