using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Enums;

namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;

public record UpdateEntranceTestModel
{
    public Guid? RoomId { get; init; }
    
    public Shift? Shift { get; init; }
    
    public DateOnly? Date { get; init; }
    public bool? IsAnnouncedScore { get; init; } 
    public string? InstructorId { get; init; }
}