using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.DayOff;

public record CreateDayOffRequest
{
    public string? Name { get; init; }
    
    [Required]
    public required DateTime StartTime { get; init; }
    
    [Required]
    public required DateTime EndTime { get; init; }
}