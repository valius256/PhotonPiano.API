namespace PhotonPiano.BusinessLogic.BusinessModel.Tuition;

public record UpdateTuitionSystemConfigModel
{
    
    public int? SlotTrial { get; init; }
    public int? DeadlineForPayTuition { get; init; }
    public decimal? TaxRates { get; init; }
}