using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.EntranceTestStudent;

public class CreateEntranceTestStudentRequest
{
    public required string StudentFirebaseId { get; set; }
    public required Guid EntranceTestId { get; set; }

    [Range(1, 10, ErrorMessage = "The range of Band Score should be between 1 and 10")]
    public decimal? BandScore { get; set; }

    [Range(1, 5, ErrorMessage = "The range of Band Score should be between 1 and 5")]
    public int? Rank { get; set; }

    public int? Year { get; set; }
    public bool IsScoreAnnounced { get; set; } = false;
}