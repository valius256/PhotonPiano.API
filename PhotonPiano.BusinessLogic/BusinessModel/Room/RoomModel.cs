using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Room;

public class RoomModel
{
    public string? Name { get; set; }
    public RoomStatus Status { get; set; } = RoomStatus.Opened;
    public int? Capacity { get; set; }
    
    public required string CreatedById { get; set; }
}