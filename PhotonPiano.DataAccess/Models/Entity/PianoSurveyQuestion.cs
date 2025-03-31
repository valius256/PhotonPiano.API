namespace PhotonPiano.DataAccess.Models.Entity;

public class PianoSurveyQuestion : BaseEntityWithId
{
    public Guid SurveyId { get; set; }

    public Guid QuestionId { get; set; }

    public int OrderIndex { get; set; }

    public bool IsRequired { get; set; }
    
    //Navigators
    public virtual PianoSurvey Survey { get; set; } = default!;

    public virtual SurveyQuestion Question { get; set; } = default!;
}