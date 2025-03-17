using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.Survey;

public record CreateLearnerAnswerRequest
{
    [Required]
    public required Guid SurveyQuestionId { get; init; }

    [Required]
    public required List<string> Answers { get; init; }
}