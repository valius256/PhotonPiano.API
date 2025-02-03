using PhotonPiano.DataAccess.Models.Enum;

public record RoomWithNameModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public RoomStatus Status { get; set; }
}