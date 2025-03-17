namespace PhotonPiano.DataAccess.Models.Entity;

public class SurveyQuestion : BaseEntityWithId
{
    public string QuestionContent { get; set; } = default!;
    public List<string> Options { get; set; } = default!;
    public bool AllowMultipleAnswers { get; set; }
    //
    public required string CreatedById { get; set; }
    public string? UpdatedById { get; set; }
    
    // reference 
    public virtual Account CreatedBy { get; set; } = default!;
    public virtual Account UpdatedBy { get; set; } = default!;
    public virtual ICollection<LearnerSurvey> LearnerSurveys { get; set; } = default!;
}