﻿using System.ComponentModel.DataAnnotations;
using PhotonPiano.Shared.Enums;

namespace PhotonPiano.Api.Requests.EntranceTest;

public record AutoArrangeEntranceTestsRequest : IValidatableObject
{
    [Required(ErrorMessage = "Learner ids are required.")]
    public required List<string> StudentIds { get; init; }

    [Required(ErrorMessage = "Start date is required.")]
    public required DateTime StartDate { get; init; }

    public DateTime? EndDate { get; init; } = null;
    public List<Shift> ShiftOptions { get; init; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate < DateTime.UtcNow.AddHours(7))
        {
            yield return new ValidationResult("Start date must be in the future.", [nameof(StartDate)]);
        }

        if (EndDate.HasValue && EndDate < DateTime.UtcNow.AddHours(7))
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