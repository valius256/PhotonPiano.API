

namespace PhotonPiano.BusinessLogic.BusinessModel.SystemConfig
{
    public record UpdateSystemConfigModel
    {
        public required string ConfigName { get; init; }
        public required string ConfigValue { get; init; }
    }
}
