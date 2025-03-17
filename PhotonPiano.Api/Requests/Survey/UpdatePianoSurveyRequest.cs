namespace PhotonPiano.Api.Requests.Survey;

public record UpdatePianoSurveyRequest
{
    public string? Name { get; init; }
    
    public string? Description { get; init; }
}