namespace PhotonPiano.BusinessLogic.BusinessModel.Survey;

public record LearnerSurveyModel
{
    public required string LearnerId { get; init; }
    
    public Guid PianoSurveyId { get; init; }

    public required string LearnerEmail { get; init; }
}