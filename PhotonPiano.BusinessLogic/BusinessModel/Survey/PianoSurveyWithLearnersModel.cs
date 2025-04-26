namespace PhotonPiano.BusinessLogic.BusinessModel.Survey;

public record PianoSurveyWithLearnersModel : PianoSurveyModel
{
    public ICollection<LearnerSurveyModel> LearnerSurveys { get; init; } = [];
}