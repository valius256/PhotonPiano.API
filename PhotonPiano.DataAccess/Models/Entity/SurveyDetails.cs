namespace PhotonPiano.DataAccess.Models.Entity;

public class SurveyDetails : BaseEntityWithId
{
    public string? LearnerId  { get; set; } 
    public string?  LearnerEmail { get; set; }
    public string SurveyName { get; set; } = default!;
    public required Guid SurveyQuestionId { get; set; }
    public string QuestionContent { get; set; } = default!;
    public List<string> Answers { get; set; } = default!;
    public List<string> Options { get; set; } = default!;
    public bool AllowMultipleAnswers { get; set; }
    public DateTime? AnswerAt { get; set; }
    // reference
    public virtual Account Learner { get; set; } = default!;
    public virtual SurveyQuestion SurveyQuestion { get; set; } = default!;
}