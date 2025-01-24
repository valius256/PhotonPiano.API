

using PhotonPiano.BusinessLogic.BusinessModel.SystemConfig;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.BusinessLogic.Interfaces
{
    public interface ISystemConfigService
    {
        Task<SystemConfig> GetConfig(string name);
        Task SetConfigValue(UpdateSystemConfigModel updateSystemConfigModel);
    }
}
