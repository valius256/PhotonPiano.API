using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.Slot;
using PhotonPiano.BusinessLogic.BusinessModel.Survey;
using PhotonPiano.BusinessLogic.BusinessModel.SystemConfig;
using PhotonPiano.BusinessLogic.BusinessModel.Tuition;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ISystemConfigService
{
    Task<List<SystemConfigModel>> GetConfigs(params List<string> names);
    Task<SystemConfigModel> GetConfig(string name);
    Task SetConfigValue(UpdateSystemConfigModel updateSystemConfigModel);
    Task<GetSystemConfigOnLevelModel> GetSystemConfigValueBaseOnLevel(int level);
    Task<SystemConfigModel?> GetTaxesRateConfig(int year);
    Task<SystemConfigsModel> GetSystemConfigs(string name);
    Task<SystemConfigModel> UpsertSystemConfig(string configName, SystemConfigType type, string value);
    Task UpdateSurveySystemConfig(UpdateSurveySystemConfigModel updateModel);
    Task UpdateEntranceTestSystemConfig(UpdateEntranceTestSystemConfigModel updateModel);
    
    Task UpdateTuitionSystemConfig(UpdateTuitionSystemConfigModel updateModel);
    
    Task UpdateSchedulerSystemConfig(UpdateSchedulerSystemConfigModel updateModel);
    Task<List<SystemConfigModel>> GetAllSurveyConfigs();
    Task<List<SystemConfigModel>> GetEntranceTestConfigs(params List<string> configNames);
}