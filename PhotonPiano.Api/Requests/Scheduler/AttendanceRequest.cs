namespace PhotonPiano.Api.Requests.Scheduler;


public record AttendanceRequest
{
    public required Guid SlotId { get; init; }
    public List<string>? StudentAttentIds { get; init; }
    public List<string>? StudentAbsentIds { get; init; }

}