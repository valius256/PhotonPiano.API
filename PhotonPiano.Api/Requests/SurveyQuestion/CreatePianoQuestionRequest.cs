using System.ComponentModel.DataAnnotations;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.SurveyQuestion;

public record CreatePianoQuestionRequest
{
    [Required]
    public required string QuestionContent { get; init; }
    
    [Required]
    [EnumDataType(typeof(QuestionType))]
    public required QuestionType Type { get; init; }

    public List<string> Options { get; init; } = [];

    public bool AllowOtherAnswer { get; init; } = false;
}