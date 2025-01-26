using PhotonPiano.Api.Responses.Class;
using PhotonPiano.Api.Responses.Room;

namespace PhotonPiano.Api.Responses.Slot;


public record GetSlotDetailResponse : SlotResponses
{
    public RoomResponse Room { get; init; } = default!;

    public ClassResponse Class { get; init; } = default!;

    public int NumberOfStudents { get; init; } = 0;


}