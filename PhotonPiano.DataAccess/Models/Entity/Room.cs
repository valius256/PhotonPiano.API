using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.Models.Entity;

public class Room : BaseEntityWithId
{
    public string? Name { get; set; }
    public RoomStatus Status { get; set; } = RoomStatus.Opened;
    public int? Capacity { get; set; }
    
    public virtual ICollection<EntranceTest> EntranceTests { get; set; } = new List<EntranceTest>();
}