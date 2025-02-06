using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.Models.Entity;

public class SlotStudent : BaseEntity
{
    public required Guid SlotId { get; set; }
    public required string StudentFirebaseId { get; set; }

    public AttendanceStatus AttendanceStatus { get; set; } = AttendanceStatus.NotYet;
    public string? TeacherNote { get; set; } = string.Empty;
    public required string CreatedById { get; set; }
    public string? UpdateById { get; set; }

    public string? DeletedById { get; set; }

    // reference 
    public virtual Slot Slot { get; set; } = default!;
    public virtual Account StudentAccount { get; set; } = default!;

    public virtual Account CreateBy { get; set; } = default!;

    public virtual Account UpdateBy { get; set; } = default!;

    public virtual Account DeletedBy { get; set; } = default!;
}