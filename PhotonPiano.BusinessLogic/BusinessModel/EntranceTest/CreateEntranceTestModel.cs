using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Enums;

namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;

public class CreateEntranceTestModel
{
    public required string Name { get; init; }
    public required Guid RoomId { get; init; }
    public required Shift Shift { get; init; }
    public required DateOnly Date { get; init; }
    public bool? IsAnnouncedScore { get; init; }
    public string? InstructorId { get; init; }

    public List<string> StudentIds { get; init; } = [];
}