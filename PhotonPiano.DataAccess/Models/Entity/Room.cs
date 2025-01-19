using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.Models.Entity;

public class Room : BaseEntityWithId
{
    public string? Name { get; set; }
    public RoomStatus Status { get; set; } = RoomStatus.Opened;
    public int? Capacity { get; set; }

    public required string CreatedById { get; set; }
    public string? UpdateById { get; set; }
    public string? DeletedById { get; set; }


    // reference 

    public virtual Account CreatedBy { get; set; } = default!;
    public virtual Account UpdateBy { get; set; } = default!;
    public virtual Account DeletedBy { get; set; } = default!;
    public virtual ICollection<EntranceTest> EntranceTests { get; set; } = new List<EntranceTest>();
    public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();
}