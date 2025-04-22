using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.Tution;

public record UpdateTuitionSystemConfigRequest   
{
    [Range(0, 10, ErrorMessage = "Number of trial slots must be between 0 and 10")]
    public int? SlotTrial { get; init; }

    [Range(1, 31, ErrorMessage = "Tuition payment deadline must be between day 1 and 31")]
    public int? DeadlineForPayTuition { get; init; }

    [Range(0, 1, ErrorMessage = "Tax rates must be between 0 and 1 (e.g., 0.1 for 10%)")]
    [RegularExpression(@"^\d*\.?\d{1,2}$", ErrorMessage = "Tax rates must have maximum 2 decimal places")]
    public decimal? TaxRates { get; init; }

}