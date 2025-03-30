using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;

public record UpdateStudentsEntranceTestResultsRequest
{
    [Required]
    public required List<UpdateStudentEntranceTestResultsRequest> UpdateRequests { get; init; }
}

public record UpdateStudentEntranceTestResultsRequest
{
    [Required] public required string StudentId { get; init; }

    [Required] public required decimal TheoraticalScore { get; init; }

    public string? InstructorComment { get; init; }

    [Required] public required List<UpdateEntranceTestScoreRequest> Scores { get; init; }
}

public record UpdateEntranceTestScoreRequest
{
    [Required(ErrorMessage = "Criteria id is required")]
    public required Guid CriteriaId { get; init; }

    [Required(ErrorMessage = "Score is required")]
    [Range(0, 10, ErrorMessage = "Score must be between 0 and 10")]
    public required decimal Score { get; init; }
}