namespace PhotonPiano.BusinessLogic.BusinessModel.Criteria;

public record MinimalCriteriaModel
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }
    
    public string? Description { get; init; }

    public decimal Weight { get; init; }
}