namespace PhotonPiano.BusinessLogic.BusinessModel.Survey;

public record LearnerSurveyModel
{

    public required string LearnerId { get; init; }

    public required string LearnerEmail { get; init; }

    public required Guid SurveyQuestionId { get; init; }

    public required string QuestionContent { get; init; }

    public List<string> Answers { get; init; } = [];

    public List<string> Options { get; init; } = [];

    public bool AllowMultipleAnswers { get; init; }
}