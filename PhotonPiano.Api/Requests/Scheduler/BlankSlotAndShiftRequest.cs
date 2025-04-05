namespace PhotonPiano.Api.Requests.Scheduler;

public record BlankSlotAndShiftRequest
{
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
}