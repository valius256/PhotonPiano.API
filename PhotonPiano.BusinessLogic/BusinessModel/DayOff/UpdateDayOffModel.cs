namespace PhotonPiano.BusinessLogic.BusinessModel.DayOff;

public record UpdateDayOffModel
{
    public string? Name { get; init; }
    
    public DateTime? StartTime { get; init; }
    
    public DateTime? EndTime { get; init; }
}