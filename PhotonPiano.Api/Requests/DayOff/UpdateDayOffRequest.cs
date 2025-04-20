namespace PhotonPiano.Api.Requests.DayOff;

public record UpdateDayOffRequest
{
    public string? Name { get; init; }
    
    public DateTime? StartTime { get; init; }
    
    public DateTime? EndTime { get; init; }
}