using PhotonPiano.BusinessLogic.BusinessModel.SurveyQuestion;

namespace PhotonPiano.BusinessLogic.BusinessModel.Survey;

public record CreatePianoSurveyModel
{
    public required string Name { get; init; }

    public string? Description { get; init; }
    
    public int? MinAge { get; init; }

    public int? MaxAge { get; init; }
    
    public HashSet<CreateQuestionInSurveyModel> CreateQuestionRequests { get; init; } = [];
}