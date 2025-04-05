namespace PhotonPiano.BusinessLogic.BusinessModel.Survey;

public record PianoSurveyModel : BaseModel
{
    public required Guid Id { get; init; }
    
    public required string Name { get; init; }

    public string? Description { get; init; }

    public string CreatedById { get; init; } = default!;

    public string? UpdatedById { get; init; } = default;
    
    public int? MinAge { get; init; }

    public int? MaxAge { get; init; }

    public bool IsEntranceSurvey { get; set; } = false;
}