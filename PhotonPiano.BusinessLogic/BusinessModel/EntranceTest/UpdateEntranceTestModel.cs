using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;

public record UpdateEntranceTestModel
{
    public Guid? RoomId { get; init; }
    public string? RoomName { get; init; }
    public int? RoomCapacity { get; init; }
    public Shift? Shift { get; init; }
    public DateOnly? StartTime { get; init; }
    public bool? IsAnnouncedTime { get; init; }
    public DateTime? AnnouncedTime { get; init; }
    public bool? IsAnnouncedScore { get; init; }
    public string? InstructorId { get; init; }
    public string? InstructorName { get; init; }
}