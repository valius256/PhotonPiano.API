using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Responses.Room;

public class RoomResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public RoomStatus Status { get; set; }
    public int? Capacity { get; set; }

    public string? CreatedById { get; set; }
}