using PhotonPiano.BusinessLogic.BusinessModel.SystemConfig;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ISystemConfigService
{
    Task<List<SystemConfigModel>> GetAllConfigs();
    Task<SystemConfigModel> GetConfig(string name);
    Task SetConfigValue(UpdateSystemConfigModel updateSystemConfigModel);
    Task<GetSystemConfigOnLevelModel> GetSystemConfigValueBaseOnLevel(int level);
    Task<SystemConfigModel?> GetTaxesRateConfig(int year);
    Task<SystemConfigsModel> GetSystemConfigs(string name);
}