using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;

public record EntranceTestModel
{
    public required Guid Id { get; set; }
    public required Guid RoomId { get; set; }   
    public string?  RoomName { get; set; }
    public int? RoomCapacity { get; set; }
    public required Shift Shift { get; set; }
    public required DateTime StartTime { get; set; }
    public bool IsAnnouncedTime { get; set; } = false;
    public required DateTime AnnouncedTime { get; set; }  
    public bool IsAnnouncedScore { get; set; } = false;
    public string? InstructorId { get; init; }
    public string? InstructorName { get; init; }
    
    public required string CreatedById { get; set; }
    public string? UpdateById { get; set; }
    public string? DeletedById { get; set; }
}