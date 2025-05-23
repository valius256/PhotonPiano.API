using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.EntranceTest;

public record UpdateEntranceTestScoreAnnouncementRequest
{
    [Required]
    public required bool IsAnnounced { get; init; }
}