using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Class;

public record ClassSimpleModel
{
    public required Guid Id { get; init; }
    public string? InstructorId { get; init; }
    public Level? Level { get; init; }
    public required string Name { get; init; }
}