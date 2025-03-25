using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.SystemConfig;

public record SystemConfigsModel
{
    public required Guid Id { get; set; }
    public required string ConfigName { get; set; }
    public List<string>? ConfigValue { get; set; }
    public Role Role { get; set; }
}