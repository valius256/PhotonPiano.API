using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.Survey;
using PhotonPiano.BusinessLogic.BusinessModel.SystemConfig;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ISystemConfigService
{
    Task<List<SystemConfigModel>> GetAllConfigs();
    Task<SystemConfigModel> GetConfig(string name);
    Task SetConfigValue(UpdateSystemConfigModel updateSystemConfigModel);
    Task<GetSystemConfigOnLevelModel> GetSystemConfigValueBaseOnLevel(int level);
    Task<SystemConfigModel?> GetTaxesRateConfig(int year);
    Task<SystemConfigsModel> GetSystemConfigs(string name);
    Task<SystemConfigModel> UpsertSystemConfig(string configName, SystemConfigType type, string value);
    Task UpdateSurveySystemConfig(UpdateSurveySystemConfigModel updateModel);
    Task UpdateEntranceTestSystemConfig(UpdateEntranceTestSystemConfigModel updateModel);
    Task<List<SystemConfigModel>> GetAllSurveyConfigs();
    Task<List<SystemConfigModel>> GetEntranceTestConfigs(params List<string> configNames);
}