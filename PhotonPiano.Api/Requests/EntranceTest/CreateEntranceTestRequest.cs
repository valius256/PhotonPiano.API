using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.EntranceTest;

public record CreateEntranceTestRequest
{
    public required Guid RoomId { get; set; }
    public string? RoomName { get; set; }
    public int? RoomCapacity { get; set; }
    public Shift? Shift { get; set; }
    public DateTime? StartTime { get; set; }
    public bool? IsAnnouncedScore { get; set; }
    public string? InstructorId { get; set; }
    public string? InstructorName { get; set; }
    public bool? IsOpen { get; set; }
}