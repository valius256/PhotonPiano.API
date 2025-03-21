using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.Scheduler;

public class PublicNewSlotRequest
{
    public Guid RoomId { get; init; }
    public DateOnly Date { get; init; }
    public Shift Shift { get; init; }
    public Guid ClassId { get; init; }
}