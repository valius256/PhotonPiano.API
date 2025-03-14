using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Responses.Class;

public record ClassLevelResponse
{
    public Guid Id { get; set; }
    public LevelEnum Level { get; set; }
    public required string Name { get; init; }
}