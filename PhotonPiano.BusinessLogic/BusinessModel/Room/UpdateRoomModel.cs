using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Room;

public class UpdateRoomModel
{
    public string? Name { get; set; }
    public RoomStatus Status { get; set; } = RoomStatus.Opened;
    public int? Capacity { get; set; }

}