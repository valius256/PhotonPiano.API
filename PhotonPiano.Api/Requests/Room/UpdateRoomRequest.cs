using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.Room;

public class UpdateRoomRequest
{
    public string? Name { get; set; }
    public RoomStatus Status { get; set; }
    public int? Capacity { get; set; }
}