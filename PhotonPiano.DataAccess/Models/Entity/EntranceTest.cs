using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.Models.Entity;

public class EntranceTest : BaseEntityWithId
{
    public required Guid RoomId { get; set; }   
    public required Shift Shift { get; set; }
    public required DateTime StartTime { get; set; }
    public bool IsAnnouncedTime { get; set; } = false;
    public required DateTime AnnouncedTime { get; set; }  
    public bool IsAnnouncedScore { get; set; } = false;
    public required string TeacherFirebaseId { get; set; }
    
    
    // reference
    public virtual Room Room { get; set; } 
    public virtual Account TeacherAccount { get; set; } 
    public virtual ICollection<EntranceTestStudent> EntranceTestStudents { get; set; } = new List<EntranceTestStudent>();
}