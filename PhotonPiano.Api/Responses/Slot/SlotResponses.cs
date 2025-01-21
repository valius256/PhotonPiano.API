using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Responses.Slot;

public class SlotResponses
{
    public Guid? ClassId { get; set; }
    public Guid? RoomId { get; set; }
    public Shift Shift { get; set; }
    public DateOnly Date { get; set; }
    public SlotStatus Status { get; set; } 
}