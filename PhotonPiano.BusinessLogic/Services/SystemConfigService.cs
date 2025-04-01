using Mapster;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.Survey;
using PhotonPiano.BusinessLogic.BusinessModel.SystemConfig;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Exceptions;
using PhotonPiano.Shared.Utils;

namespace PhotonPiano.BusinessLogic.Services;

public class SystemConfigService : ISystemConfigService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly List<string> _surveyConfigNames =
    [
        ConfigNames.InstrumentName, ConfigNames.InstrumentFrequencyInResponse, ConfigNames.MaxQuestionsPerSurvey,
        ConfigNames.MinQuestionsPerSurvey
    ];

    private readonly List<string> _entranceTestConfigNames =
    [
        ConfigNames.MinStudentsInTest, ConfigNames.MaxStudentsInTest, ConfigNames.AllowEntranceTestRegistering
    ];

    public SystemConfigService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<SystemConfigModel>> GetAllConfigs()
    {
        var configs = await _unitOfWork.SystemConfigRepository.GetAllAsync(hasTrackings: false);
        return configs.Adapt<List<SystemConfigModel>>();
    }

    public async Task<SystemConfigModel> GetConfig(string name)
    {
        var config = await _unitOfWork.SystemConfigRepository.FindFirstAsync(c => c.ConfigName == name);
        if (config is null) throw new NotFoundException("Config not found");
        return config.Adapt<SystemConfigModel>();
    }

    public async Task SetConfigValue(UpdateSystemConfigModel updateSystemConfigModel)
    {
        var config = await GetConfig(updateSystemConfigModel.ConfigName);
        config.ConfigValue = updateSystemConfigModel.ConfigValue;

        await _unitOfWork.SystemConfigRepository.UpdateAsync(config.Adapt<SystemConfig>());
        await _unitOfWork.SaveChangesAsync();
    }

    [Obsolete]
    public async Task<GetSystemConfigOnLevelModel> GetSystemConfigValueBaseOnLevel(int level)
    {
        var searchTerm = $" LEVEL {level}";
        var patternEnd = $"%{searchTerm}";
        var patternFollowedByNonDigit = $"%{searchTerm}[^0-9]%";
        var systemConfigs = await _unitOfWork.SystemConfigRepository
            .FindAsync(c => EF.Functions.Like(c.ConfigName.Trim(), patternEnd) ||
                            EF.Functions.Like(c.ConfigName.Trim(), patternFollowedByNonDigit));
        var configDictionary = systemConfigs.ToDictionary(c => c.ConfigName.Trim(), c => c.ConfigValue);

        var result = new GetSystemConfigOnLevelModel
        {
            NumOfSlotInWeek = configDictionary.TryGetValue($"Số buổi học 1 tuần LEVEL {level}", out var numOfSlot)
                ? int.TryParse(numOfSlot, out var resultNumOfSlotInWeek) ? resultNumOfSlotInWeek : 0
                : 0,

            TotalSlot = configDictionary.TryGetValue($"Tổng số buổi học LEVEL {level}", out var totalSlot)
                ? int.TryParse(totalSlot, out var resultNumOfTotalSlot) ? resultNumOfTotalSlot : 0
                : 0,

            PriceOfSlot = configDictionary.TryGetValue($"Mức phí theo buổi LEVEL {level}", out var priceOfSlot)
                ? int.TryParse(priceOfSlot, out var resultPriceOfSlot) ? resultPriceOfSlot : 0
                : 0
        };


        return result;
    }

    public async Task<SystemConfigModel?> GetTaxesRateConfig(int year)
    {
        while (year >= 2000)
        {
            var config = await _unitOfWork.SystemConfigRepository.FindFirstAsync(
                x => x.ConfigName == $"Thuế suất năm {year}");

            if (config != null) return config.Adapt<SystemConfigModel>();

            year--;
        }

        return null;
    }

    public async Task<SystemConfigsModel> GetSystemConfigs(string name)
    {
        var config = await _unitOfWork.SystemConfigRepository.FindFirstAsync(c => c.ConfigName == name);
        if (config is null) throw new NotFoundException("Config not found");

        List<string>? configValues = null;
        if (!string.IsNullOrEmpty(config.ConfigValue))
        {
            configValues = JsonConvert.DeserializeObject<List<string>>(config.ConfigValue);
        }

        var configModel = config.Adapt<SystemConfigsModel>();
        configModel.ConfigValue = configValues;

        return configModel;
    }

    public async Task UpdateSurveySystemConfig(UpdateSurveySystemConfigModel updateModel)
    {
        var (instrumentName, instrumentFrequencyInResponse, entranceSurveyId, maxQuestionsPerSurvey,
            minQuestionsPerSurvey) = updateModel;

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            if (!string.IsNullOrEmpty(instrumentName))
            {
                await UpsertSystemConfig(ConfigNames.InstrumentName, SystemConfigType.Text, instrumentName);
            }

            if (instrumentFrequencyInResponse.HasValue)
            {
                await UpsertSystemConfig(ConfigNames.InstrumentFrequencyInResponse, SystemConfigType.UnsignedInt,
                    instrumentFrequencyInResponse.Value.ToString());
            }

            if (!string.IsNullOrEmpty(entranceSurveyId))
            {
                if (!await _unitOfWork.PianoSurveyRepository.AnyAsync(s => s.Id.ToString() == entranceSurveyId))
                {
                    throw new NotFoundException("Entrance survey not found");
                }

                await UpsertSystemConfig(ConfigNames.EntranceSurvey, SystemConfigType.Text, entranceSurveyId);
            }

            if (maxQuestionsPerSurvey.HasValue)
            {
                await UpsertSystemConfig(ConfigNames.MaxQuestionsPerSurvey, SystemConfigType.UnsignedInt,
                    maxQuestionsPerSurvey.Value.ToString());
            }

            if (minQuestionsPerSurvey.HasValue)
            {
                await UpsertSystemConfig(ConfigNames.MinQuestionsPerSurvey, SystemConfigType.UnsignedInt,
                    minQuestionsPerSurvey.Value.ToString());
            }
        });
    }

    public async Task UpdateEntranceTestSystemConfig(UpdateEntranceTestSystemConfigModel updateModel)
    {
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            if (updateModel.MinStudentsPerEntranceTest.HasValue)
            {
                await UpsertSystemConfig(ConfigNames.MinStudentsInTest, SystemConfigType.UnsignedInt,
                    updateModel.MinStudentsPerEntranceTest.Value.ToString());
            }

            if (updateModel.MaxStudentsPerEntranceTest.HasValue)
            {
                await UpsertSystemConfig(ConfigNames.MaxStudentsInTest, SystemConfigType.UnsignedInt,
                    updateModel.MaxStudentsPerEntranceTest.Value.ToString());
            }

            if (updateModel.AllowEntranceTestRegistering.HasValue)
            {
                await UpsertSystemConfig(ConfigNames.AllowEntranceTestRegistering, SystemConfigType.Boolean,
                    updateModel.AllowEntranceTestRegistering.Value.ToString());
            }
        });
    }

    public async Task<List<SystemConfigModel>> GetAllSurveyConfigs()
    {
        var surveyConfigs = await _unitOfWork.SystemConfigRepository.FindProjectedAsync<SystemConfigModel>(
            expression: c =>
                _surveyConfigNames.Contains(c.ConfigName),
            hasTrackings: false);

        return surveyConfigs;
    }

    public async Task<List<SystemConfigModel>> GetAllEntranceTestConfigs()
    {
        return await _unitOfWork.SystemConfigRepository.FindProjectedAsync<SystemConfigModel>(
            expression: c =>
                _entranceTestConfigNames.Contains(c.ConfigName),
            hasTrackings: false);
    }

    public async Task<SystemConfigModel> UpsertSystemConfig(string configName, SystemConfigType type, string value)
    {
        var config = await _unitOfWork.SystemConfigRepository.FindFirstAsync(c => c.ConfigName == configName);

        if (config is not null)
        {
            config.ConfigValue = value;
            config.UpdatedAt = DateTime.UtcNow.AddHours(7);

            await _unitOfWork.SystemConfigRepository.UpdateAsync(config);

            return config.Adapt<SystemConfigModel>();
        }

        var dbConfig = new SystemConfig
        {
            Id = Guid.NewGuid(),
            ConfigName = configName,
            ConfigValue = value,
            Type = type
        };

        await _unitOfWork.SystemConfigRepository.AddAsync(dbConfig);

        return dbConfig.Adapt<SystemConfigModel>();
    }
}