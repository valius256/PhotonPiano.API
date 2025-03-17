using System.Collections;

namespace PhotonPiano.DataAccess.Models.Entity;

public class LearnerSurvey : BaseEntityWithId
{
    public required string LearnerId { get; set; }
    public Guid PianoSurveyId { get; set; }
    public required string LearnerEmail { get; set; }
    
    // reference
    public virtual Account Learner { get; set; } = default!;
    public virtual PianoSurvey PianoSurvey { get; set; } = default!;
    public virtual ICollection<LearnerAnswer> LearnerAnswers { get; set; } = [];
    
}