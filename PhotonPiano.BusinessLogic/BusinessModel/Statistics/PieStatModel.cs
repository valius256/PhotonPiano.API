namespace PhotonPiano.BusinessLogic.BusinessModel.Statistics;

public record PieStatModel
{
    public required string Name { get; init; }

    public double Percentage { get; init; }

    public double Value { get; init; }

    public string? Color { get; init; }
}