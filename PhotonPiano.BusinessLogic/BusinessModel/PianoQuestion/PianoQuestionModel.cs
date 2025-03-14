using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.PianoQuestion;

public record PianoQuestionModel
{
    public required Guid Id { get; init; }
    
    public required string QuestionContent { get; init; }

    public List<string> Options { get; init; } = []; //Only for single/multiple-choice & Likert scale

    public required QuestionType Type { get; init; }

    public bool AllowOtherAnswer { get; init; }

    public required string CreatedById { get; init; }
    
    public string? UpdatedById { get; init; }
    
    public DateTime CreatedAt { get; init; } 
}