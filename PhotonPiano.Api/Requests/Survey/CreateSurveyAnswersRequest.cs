using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.Survey;

public record CreateSurveyAnswersRequest
{
    [Required]
    public required List<CreateLearnerAnswerRequest> CreateAnswerRequests { get; init; }
}