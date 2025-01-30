namespace PhotonPiano.BusinessLogic.BusinessModel.SystemConfig;

public record SystemConfigSimpleModel
{
    public Guid Id { get; init; }
    public string ConfigName { get; init; }
    public string ConfigValue { get; init; }
}