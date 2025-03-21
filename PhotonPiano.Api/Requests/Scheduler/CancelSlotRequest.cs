namespace PhotonPiano.Api.Requests.Scheduler;

public record CancelSlotRequest
{
    public Guid SlotId { get; init; }
    public required string CancelReason { get; init; }
}