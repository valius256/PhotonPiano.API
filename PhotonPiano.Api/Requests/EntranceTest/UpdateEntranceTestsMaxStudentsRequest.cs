using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.EntranceTest;

public record UpdateEntranceTestsMaxStudentsRequest
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Max students must be greater than 0")]
    public required int MaxStudents { get; init; }
}