using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.EntranceTestStudent;

public class UpdateEntranceTestStudentRequest
{
    public required string StudentFirebaseId { get; init; }
    public required Guid EntranceTestId { get; init; } 
    [Range(1, 10, ErrorMessage = "The range of Band Score should be between 1 and 10")]
    public decimal? BandScore { get; set; }
    [Range(1,5, ErrorMessage = "The range of Band Score should be between 1 and 5")]
    public int? Rank { get; set; }
    public int? Year { get; init; } 
    public bool IsScoreAnnounced { get; init; }
}