namespace PhotonPiano.BusinessLogic.BusinessModel.PianoQuestion;

public record CreatePianoQuestionModel
{
    public required string QuestionContent { get; init; } 
    
    public required List<string> Options { get; init; }
    
    public bool AllowMultipleAnswers { get; init; }
    
    public bool AllowOtherAnswer { get; init; }
}