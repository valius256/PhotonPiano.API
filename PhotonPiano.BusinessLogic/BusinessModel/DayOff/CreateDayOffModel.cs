
namespace PhotonPiano.BusinessLogic.BusinessModel.DayOff;

public record CreateDayOffModel
{
    public string? Name { get; init; }
    
    public required DateTime StartTime { get; init; }
    
    public required DateTime EndTime { get; init; }
}