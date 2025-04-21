using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.Tution;

public record UpdateTuitionSystemConfigRequest   
{
    [Range(1, int.MaxValue, ErrorMessage = "Number of slot trial must > 0.")]
    public int? SlotTrial { get; init; }
    
    [Range(1, 30, ErrorMessage = "Number of Deadline for pay tuition must > 0.")]
    public int? DeadlineForPayTuition { get; init; }
    [Range(0, 1, ErrorMessage = "Number of tax rates must > 0.")]
    public decimal? TaxRates { get; init; }
}