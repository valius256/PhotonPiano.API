namespace PhotonPiano.BusinessLogic.BusinessModel.Survey;

public record UpdatePianoSurveyModel
{
    public string? Name { get; init; }
    
    public string? Description { get; init; }
}