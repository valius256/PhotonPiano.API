using PhotonPiano.Shared.Enums;

namespace PhotonPiano.BusinessLogic.BusinessModel.Statistics;

public record StatModel
{
    public required string Name { get; init; }

    public int? Month { get; init; }

    public int? Year { get; init; }

    public required decimal Value { get; init; }

    public StatUnit Unit { get; init; }

    public decimal? ValueCompareToLastMonth { get; init; }
}