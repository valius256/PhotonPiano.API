namespace PhotonPiano.DataAccess.Models.Entity;

public class LearnerAnswer : BaseEntityWithId
{
    public Guid LearnerSurveyId { get; set; }

    public Guid SurveyQuestionId { get; set; }

    public List<string> Answers { get; set; } = [];
    
    //Navigators
    public virtual LearnerSurvey LearnerSurvey { get; set; } = default!;

    public virtual SurveyQuestion SurveyQuestion { get; set; } = default!;
}