namespace PhotonPiano.DataAccess.Models.Entity;

public class LearnerSurvey : BaseEntity
{
    public required string LearnerId { get; set; } 
    
    public required string LearnerEmail { get; set; }
    public required Guid SurveyQuestionId { get; set; }
    
    public required string QuestionContent { get; set; }

    public List<string> Answers { get; set; } = [];

    public List<string> Options { get; set; } = [];
    
    public bool AllowMultipleAnswers { get; set; }
    // reference
    public virtual Account Account { get; set; } = default!;
    public virtual PianoQuestion PianoQuestion { get; set; } = default!;
}