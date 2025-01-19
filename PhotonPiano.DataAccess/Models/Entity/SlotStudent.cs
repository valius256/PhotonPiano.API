using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.Models.Entity;

public class SlotStudent : BaseEntity
{
    public required Guid SlotId { get; set; }
    public required string StudentFirebaseId { get; set; }

    public AttemptStatus AttemptStatus { get; set; } = AttemptStatus.NotYet;

    public required string CreatedById { get; set; }
    public string? UpdateById { get; set; }

    public string? DeletedById { get; set; }

    // reference 
    public virtual required Slot Slot { get; set; } = default!;
    public virtual required Account StudentAccount { get; set; } = default!;

    public virtual required Account CreateBy { get; set; } = default!;

    public virtual required Account UpdateBy { get; set; } = default!;

    public virtual required Account DeletedBy { get; set; } = default!;
}