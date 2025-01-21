using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Responses.Room;

public record RoomSimpleResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public RoomStatus Status { get; set; }
}