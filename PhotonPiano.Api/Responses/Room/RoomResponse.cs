using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Responses.Room;

public record RoomResponse
{
    public Guid Id { get; init; }
    
    public string? Name { get; init; }
    
    public RoomStatus Status { get; init; }
    
    public int? Capacity { get; init; }
    
    public string? CreatedById { get; init; }
}