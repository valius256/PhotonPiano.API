using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;

public record EntranceTestModel
{
    public Guid Id { get; init; }
    public Guid RoomId { get; init; }   
    public Shift Shift { get; init; }
    public DateTime StartTime { get; init; }
    public bool IsAnnouncedTime { get; init; } 
    public DateTime AnnouncedTime { get; init; }  
    public bool IsAnnouncedScore { get; init; }
    public string TeacherFirebaseId { get; init; }
}