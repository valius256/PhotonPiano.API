using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Class;

public record ClassSimpleModel
{
    public required Guid Id { get; init; }
    public string? InstructorId { get; init; }
    public string? InstructorName { get; init; }
    public Guid? LevelId { get; init; }
    public required string Name { get; init; }
}