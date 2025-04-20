using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.EntranceTest;

public record UpdateEntranceTestResultsRequest
{
    public string? InstructorComment { get; init; }

    public Guid? LevelId { get; init; }

    [Range(0, 10, ErrorMessage = "Score must be between 0 and 10")]
    public double? TheoraticalScore { get; init; }

    public List<UpdateEntranceTestScoreRequest> UpdateScoreRequests { get; init; } = [];
}

public record UpdateEntranceTestScoreRequest
{
    [Required(ErrorMessage = "Criteria id is required")] public required Guid CriteriaId { get; init; }

    [Required(ErrorMessage = "Score is required")]
    [Range(0, 10, ErrorMessage = "Score must be between 0 and 10")]
    public required decimal Score { get; init; }
}