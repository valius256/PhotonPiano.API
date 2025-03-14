using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.PianoQuestion;

public record UpdatePianoQuestionModel
{
    public QuestionType? Type { get; init; }
    
    public string? QuestionContent { get; init; }

    public List<string>? Options { get; init; }
    
    public bool? AllowOtherAnswer { get; init; }
}