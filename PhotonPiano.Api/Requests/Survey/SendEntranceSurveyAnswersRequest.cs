using System.ComponentModel.DataAnnotations;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.Survey;

public record SendEntranceSurveyAnswersRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; init; }
    
    [Required]
    public required string Password { get; init; }
    
    [Required]
    public required string FullName { get; init; }

    [Required]
    public required string Phone { get; init; }
    
    [Required] 
    public required Gender Gender { get; init; }

    [Required]
    public required List<CreateLearnerAnswerRequest> SurveyAnswers { get; init; }
}