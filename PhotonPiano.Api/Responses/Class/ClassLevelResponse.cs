using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Responses.Class;

public class ClassLevelResponse
{
    public Guid Id { get; set; }
    public Level Level { get; set; }
}