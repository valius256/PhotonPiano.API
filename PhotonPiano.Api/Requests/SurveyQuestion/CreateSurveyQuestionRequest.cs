using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.SurveyQuestion;

public record CreateSurveyQuestionRequest
{
    [Required]
    public required string QuestionContent { get; init; }

    [Required]
    public required List<string> Options { get; init; }

    public bool AllowMultipleAnswers { get; init; } = false;
}