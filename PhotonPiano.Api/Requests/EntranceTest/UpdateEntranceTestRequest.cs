using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Enums;

namespace PhotonPiano.Api.Requests.EntranceTest;

public record UpdateEntranceTestRequest
{
    public Guid? RoomId { get; init; }
    
    public Shift? Shift { get; init; }
    
    public DateOnly? Date { get; init; }
    
    public string? InstructorId { get; init; }
}