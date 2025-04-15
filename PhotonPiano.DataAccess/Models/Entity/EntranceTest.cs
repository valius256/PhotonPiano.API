using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Enums;

namespace PhotonPiano.DataAccess.Models.Entity;

public class EntranceTest : BaseEntityWithId
{
    public required string Name { get; set; }
    public required Guid RoomId { get; set; }
    public string? RoomName { get; set; }
    public int? RoomCapacity { get; set; }
    public required Shift Shift { get; set; }
    public required DateOnly Date { get; set; }
    public bool IsAnnouncedScore { get; set; } = false;
    public string? InstructorId { get; set; }
    public string? InstructorName { get; set; }
    public required string CreatedById { get; set; }
    public string? UpdateById { get; set; }
    public string? DeletedById { get; set; }
    public decimal Fee { get; set; }


    // reference
    public virtual Room Room { get; set; } = default!;
    public virtual Account Instructor { get; set; } = default!;
    public virtual Account CreatedBy { get; set; } = default!;
    public virtual Account UpdateBy { get; set; } = default!;
    public virtual Account DeletedBy { get; set; } = default!;

    public virtual ICollection<EntranceTestStudent> EntranceTestStudents { get; set; } =
        new List<EntranceTestStudent>();
}