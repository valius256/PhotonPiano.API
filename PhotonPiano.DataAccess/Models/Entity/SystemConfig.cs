using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.Models.Entity;

public class SystemConfig : BaseEntityWithId
{
    public required string ConfigName { get; set; }
    public string? ConfigValue { get; set; }
    public Role Role { get; set; }
}