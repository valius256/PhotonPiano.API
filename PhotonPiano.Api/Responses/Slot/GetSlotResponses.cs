using PhotonPiano.Api.Responses.Class;
using PhotonPiano.Api.Responses.Room;

namespace PhotonPiano.Api.Responses.Slot;

public class GetSlotResponses : SlotResponses
{
    public RoomSimpleResponse Room { get; set; }
    public ClassLevelResponse Class { get; set; }
}