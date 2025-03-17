namespace PhotonPiano.BusinessLogic.BusinessModel.Survey;

public record CreatePianoSurveyModel
{
    public required string Name { get; init; }

    public string? Description { get; init; }
}