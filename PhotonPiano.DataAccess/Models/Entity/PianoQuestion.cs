using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.Models.Entity;

public class PianoQuestion : BaseEntityWithId
{
    public required string QuestionContent { get; set; }

    public List<string> Options { get; set; } = []; //Only for single/multiple-choice & Likert scale 

    public required QuestionType Type { get; set; }
    public bool AllowOtherAnswer { get; set; }
    //
    public required string CreatedById { get; set; }
    public string? UpdatedById { get; set; }
    
    // reference 
    public virtual Account CreatedBy { get; set; } = default!;
    public virtual Account UpdatedBy { get; set; } = default!;
    public virtual ICollection<LearnerSurvey> LearnerSurveys { get; set; } = default!;
}