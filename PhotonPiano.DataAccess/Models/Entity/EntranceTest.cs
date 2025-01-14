using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.Models.Entity;

public class EntranceTest : BaseEntityWithId
{
    public required Guid RoomId { get; set; }
    public string? RoomName { get; set; }
    public int? RoomCapacity { get; set; }
    public required Shift Shift { get; set; }
    public required DateOnly StartTime { get; set; }
    public bool IsAnnouncedScore { get; set; } = false;
    public string? InstructorId { get; set; }
    public string? InstructorName { get; set; }
    public bool IsOpen { get; set; } = true;
    public required string CreatedById { get; set; }
    public string? UpdateById { get; set; }
    public string? DeletedById { get; set; }

    // reference
    public virtual Room Room { get; set; }
    public virtual Account Instructor { get; set; }
    public virtual Account CreatedBy { get; set; }
    public virtual Account UpdateBy { get; set; }
    public virtual Account DeletedBy { get; set; }

    public virtual ICollection<EntranceTestStudent> EntranceTestStudents { get; set; } =
        new List<EntranceTestStudent>();
}