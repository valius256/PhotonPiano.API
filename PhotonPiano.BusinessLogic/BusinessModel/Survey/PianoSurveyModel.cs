namespace PhotonPiano.BusinessLogic.BusinessModel.Survey;

public record PianoSurveyModel
{
    public required Guid Id { get; init; }
    
    public required string Name { get; init; }

    public string? Description { get; init; }

    public string CreatedById { get; init; } = default!;

    public string? UpdatedById { get; init; } = default;
    
    public DateTime CreatedAt { get; init; } 
    
    public DateTime? UpdatedAt { get; init; }
}