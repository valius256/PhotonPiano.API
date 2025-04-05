namespace PhotonPiano.BusinessLogic.BusinessModel.Survey;

public record LearnerSurveyWithSurveyModel : LearnerSurveyModel
{
    public PianoSurveyModel PianoSurvey { get; init; } = default!;
}