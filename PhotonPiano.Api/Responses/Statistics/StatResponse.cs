using PhotonPiano.Shared.Enums;

namespace PhotonPiano.Api.Responses.Statistics;

public record StatResponse
{
    public required string Name { get; init; }

    public double Value { get; init; }

    public StatUnit Unit { get; init; }

    public double ValueCompareToLastMonth { get; init; }
}