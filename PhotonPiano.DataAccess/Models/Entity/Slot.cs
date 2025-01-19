using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.Models.Entity;

public class Slot : BaseEntityWithId
{
    public Guid? ClassId { get; set; }
    public Guid? RoomId { get; set; }
    public Shift Shift { get; set; }
    public DateOnly Date { get; set; }
    public SlotStatus Status { get; set; } = SlotStatus.NotStarted;

    // reference
    public virtual Class Class { get; set; } = default!;
    public virtual Room Room { get; set; } = default!;

    public virtual ICollection<SlotStudent> SlotStudents { get; set; } = new List<SlotStudent>();
}