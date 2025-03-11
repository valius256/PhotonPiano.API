namespace PhotonPiano.DataAccess.Models.Entity;

public class LearnerSurvey : BaseEntityWithId
{
    public string AccountId { get; set; } = default!;
    public required Guid SurveyQuestionId { get; set; }
    public string QuestionContent { get; set; } = default!;
    public List<string> Answers { get; set; } = default!;
    public List<string> Options { get; set; } = default!;
    
    // reference
    public virtual Account Account { get; set; } = default!;
    public virtual SurveyQuestion SurveyQuestion { get; set; } = default!;
}