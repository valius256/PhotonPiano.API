using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;

public record EntranceTestModel
{
    public required Guid Id { get; init; }
    public required Guid RoomId { get; init; }   
    public string?  RoomName { get; init; }
    public int? RoomCapacity { get; init; }
    public required Shift Shift { get; init; }
    public required DateTime StartTime { get; init; }
    public bool IsAnnouncedTime { get; init; } = false;
    
    public required DateTime AnnouncedTime { get; init; }  
    
    public bool IsAnnouncedScore { get; init; } = false;
    
    public string? InstructorId { get; init; }
    public string? InstructorName { get; init; }
    
    public required string CreatedById { get; init; }
    
    public string? UpdateById { get; init; }
    
    public string? DeletedById { get; init; }
    
    public RecordStatus RecordStatus { get; init; } = RecordStatus.IsActive;
}