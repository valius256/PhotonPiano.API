using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.EntranceTest;

public record UpdateEntranceTestRequest
{
    public Guid? RoomId { get; init; }   
    public string?  RoomName { get; init; }
    public int? RoomCapacity { get; init; }
    public Shift? Shift { get; init; }
    public DateTime? StartTime { get; init; }
    public bool? IsAnnouncedTime { get; init; } = false;
    public DateTime? AnnouncedTime { get; init; }  
    public bool? IsAnnouncedScore { get; init; } = false;
    public string? InstructorFirebaseId { get; init; }
    public string? InstructorName { get; init; }
}