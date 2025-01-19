using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Class;

public record ClassModel
{
    public string? InstructorId { get; init; }
    public ClassStatus Status { get; init; }
    public Level? Level { get; init; }
    public bool IsPublic { get; init; }
}