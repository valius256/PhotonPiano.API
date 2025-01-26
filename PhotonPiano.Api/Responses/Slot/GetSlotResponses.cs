using PhotonPiano.Api.Responses.Class;
using PhotonPiano.Api.Responses.Room;

namespace PhotonPiano.Api.Responses.Slot;

public record GetSlotResponses : SlotResponses
{
    public RoomSimpleResponse Room { get; init; } = default!;
    public ClassLevelResponse Class { get; init; } = default!;
}