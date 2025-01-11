using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Responses.EntranceTest;

public record EntranceTestResponse
{
    public required Guid Id { get; init; }
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow.AddHours(7);
    
    public DateTime? UpdatedAt { get; init; }
    
    public DateTime? DeletedAt { get; init; }
    
    public RecordStatus Status { get; init; } = RecordStatus.IsActive;
    
    public required Guid RoomId { get; init; }
    
    public string? RoomName { get; init; }
    
    public int? RoomCapacity { get; init; }
    
    public required Shift Shift { get; init; }
    
    public required DateTime StartTime { get; init; }
    
    public bool IsAnnouncedTime { get; init; } = false;
    
    public required DateTime AnnouncedTime { get; init; }
    
    public bool IsAnnouncedScore { get; init; } = false;
    
    public string? InstructorId { get; init; }
    
    public string? InstructorName { get; init; }

    public required string CreatedById { get; init; }

    public int RegisterStudents { get; init; }
}