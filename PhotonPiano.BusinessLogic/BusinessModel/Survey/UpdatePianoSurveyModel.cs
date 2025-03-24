using PhotonPiano.BusinessLogic.BusinessModel.SurveyQuestion;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Survey;

public record UpdatePianoSurveyModel
{
    public string? Name { get; init; }
    
    public string? Description { get; init; }
    
    public RecordStatus? RecordStatus { get; init; }
    
    public bool? IsEntranceSurvey { get; init; }
    
    public HashSet<UpdateSurveyQuestionInSurveyModel> Questions { get; init; } = [];
}