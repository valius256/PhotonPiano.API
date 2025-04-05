using PhotonPiano.Api.Requests.SurveyQuestion;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.Survey;

public record UpdatePianoSurveyRequest
{
    public string? Name { get; init; }
    
    public string? Description { get; init; }

    public RecordStatus? RecordStatus { get; init; }

    public bool? IsEntranceSurvey { get; init; }

    public HashSet<UpdateSurveyQuestionInSurveyRequest> Questions { get; init; } = [];
}