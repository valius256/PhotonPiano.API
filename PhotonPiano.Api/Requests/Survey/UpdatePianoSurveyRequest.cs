using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.Survey;

public record UpdatePianoSurveyRequest
{
    public string? Name { get; init; }
    
    public string? Description { get; init; }

    public RecordStatus? RecordStatus { get; init; }
}