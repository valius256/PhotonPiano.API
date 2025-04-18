using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Enums;

namespace PhotonPiano.Api.Requests.EntranceTest;

public record CreateEntranceTestRequest
{
    public required string Name { get; set; }
    public required Guid RoomId { get; set; }
    public Shift? Shift { get; set; }
    public DateOnly? Date { get; set; }
    public bool? IsAnnouncedScore { get; set; }
    public string? InstructorId { get; set; }

    public List<string> StudentIds { get; init; } = [];
}