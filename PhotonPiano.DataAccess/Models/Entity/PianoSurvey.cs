using System.Collections;

namespace PhotonPiano.DataAccess.Models.Entity;

public class PianoSurvey : BaseEntityWithId
{
    public required string Name { get; set; }

    public string? Description { get; set; }

    public string CreatedById { get; set; } = default!;

    public string? UpdatedById { get; set; } = default;
    
    //Navigators
    public virtual Account CreatedBy { get; set; } = default!;
    public virtual Account UpdatedBy { get; set; } = default!;
    public virtual ICollection<LearnerSurvey> LearnerSurveys { get; set; } = [];
    public virtual ICollection<SurveyQuestion> SurveyQuestions { get; set; } = [];
}