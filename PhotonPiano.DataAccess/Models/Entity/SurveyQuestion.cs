using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.Models.Entity;

public class SurveyQuestion : BaseEntityWithId
{
    public QuestionType Type { get; set; }
    
    public string QuestionContent { get; set; } = default!;
    
    public List<string> Options { get; set; } = default!;
    public int OrderIndex { get; set; }
    public bool AllowOtherAnswer { get; set; }
    
    public int MinAge { get; set; }

    public int? MaxAge { get; set; }

    public bool IsRequired { get; set; }
    public required string CreatedById { get; set; }
    public string? UpdatedById { get; set; }
    
    // reference 
    public virtual Account CreatedBy { get; set; } = default!;
    public virtual Account UpdatedBy { get; set; } = default!;
    public virtual ICollection<PianoSurvey> PianoSurveys { get; set; } = default!;
    public virtual ICollection<LearnerAnswer> LearnerAnswers { get; set; } = [];
}