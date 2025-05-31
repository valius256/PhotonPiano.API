using Mapster;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.Slot;
using PhotonPiano.BusinessLogic.BusinessModel.Survey;
using PhotonPiano.BusinessLogic.BusinessModel.SystemConfig;
using PhotonPiano.BusinessLogic.BusinessModel.Tuition;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Exceptions;
using PhotonPiano.Shared.Utils;

namespace PhotonPiano.BusinessLogic.Services;

public class SystemConfigService : ISystemConfigService
{
    private readonly List<string> _entranceTestConfigNames =
    [
        ConfigNames.MinStudentsInTest, ConfigNames.MaxStudentsInTest, ConfigNames.AllowEntranceTestRegistering,
        ConfigNames.TestFee, ConfigNames.TheoryPercentage, ConfigNames.PracticePercentage
    ];

    private readonly IServiceFactory _serviceFactory;

    private readonly List<string> _surveyConfigNames =
    [
        ConfigNames.InstrumentName, ConfigNames.InstrumentFrequencyInResponse, ConfigNames.MaxQuestionsPerSurvey,
        ConfigNames.MinQuestionsPerSurvey
    ];

    private readonly IUnitOfWork _unitOfWork;

    public SystemConfigService(IUnitOfWork unitOfWork, IServiceFactory serviceFactory)
    {
        _unitOfWork = unitOfWork;
        _serviceFactory = serviceFactory;
    }

    public async Task<List<SystemConfigModel>> GetConfigs(params List<string> names)
    {
        return await _unitOfWork.SystemConfigRepository.FindProjectedAsync<SystemConfigModel>(
            s => names.Count == 0 || names.Contains(s.ConfigName),
            false);
    }

    public async Task<SystemConfigModel> GetConfig(string name, bool hasTrackings = true, bool requiresCaching = true)
    {
        var cacheKey = $"SystemConfig-{name}";
        if (requiresCaching)
        {
            var cachedConfig = await _serviceFactory.RedisCacheService.GetAsync<SystemConfigModel>(cacheKey);

            if (cachedConfig is not null) return cachedConfig;
        }

        var config = await _unitOfWork.SystemConfigRepository.FindFirstAsync(c => c.ConfigName == name, hasTrackings);

        if (config is null) throw new NotFoundException("Config not found");

        var result = config.Adapt<SystemConfigModel>();

        if (requiresCaching) await _serviceFactory.RedisCacheService.SaveAsync(cacheKey, result, TimeSpan.FromDays(1));

        return result;
    }
 
    public async Task SetConfigValue(UpdateSystemConfigModel updateSystemConfigModel)
    {
        var config = await GetConfig(updateSystemConfigModel.ConfigName);
        config.ConfigValue = updateSystemConfigModel.ConfigValue;

        await _unitOfWork.SystemConfigRepository.UpdateAsync(config.Adapt<SystemConfig>());
        await _unitOfWork.SaveChangesAsync();

        await _serviceFactory.RedisCacheService.DeleteByPatternAsync("SystemConfig*");
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
        var config =
            await _unitOfWork.SystemConfigRepository.FindFirstAsync(x => x.ConfigName == $"Thuế suất năm {year}");

        if (config != null) return config.Adapt<SystemConfigModel>();

        return null;
    }

    public async Task<SystemConfigsModel> GetSystemConfigs(string name)
    {
        var config = await _unitOfWork.SystemConfigRepository.FindFirstAsync(c => c.ConfigName == name);
        if (config is null) throw new NotFoundException("Config not found");

        List<string>? configValues = null;
        if (!string.IsNullOrEmpty(config.ConfigValue))
            configValues = JsonConvert.DeserializeObject<List<string>>(config.ConfigValue);

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
                await UpsertSystemConfig(ConfigNames.InstrumentName, SystemConfigType.Text, instrumentName);

            if (instrumentFrequencyInResponse.HasValue)
                await UpsertSystemConfig(ConfigNames.InstrumentFrequencyInResponse, SystemConfigType.UnsignedInt,
                    instrumentFrequencyInResponse.Value.ToString());

            if (!string.IsNullOrEmpty(entranceSurveyId))
            {
                if (!await _unitOfWork.PianoSurveyRepository.AnyAsync(s => s.Id.ToString() == entranceSurveyId))
                    throw new NotFoundException("Entrance survey not found");

                await UpsertSystemConfig(ConfigNames.EntranceSurvey, SystemConfigType.Text, entranceSurveyId);
            }

            if (maxQuestionsPerSurvey.HasValue)
                await UpsertSystemConfig(ConfigNames.MaxQuestionsPerSurvey, SystemConfigType.UnsignedInt,
                    maxQuestionsPerSurvey.Value.ToString());

            if (minQuestionsPerSurvey.HasValue)
                await UpsertSystemConfig(ConfigNames.MinQuestionsPerSurvey, SystemConfigType.UnsignedInt,
                    minQuestionsPerSurvey.Value.ToString());
        });
        await _serviceFactory.RedisCacheService.DeleteByPatternAsync("SystemConfig");
    }

    public async Task UpdateEntranceTestSystemConfig(UpdateEntranceTestSystemConfigModel updateModel)
    {
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            if (updateModel.MinStudentsPerEntranceTest.HasValue)
                await UpsertSystemConfig(ConfigNames.MinStudentsInTest, SystemConfigType.UnsignedInt,
                    updateModel.MinStudentsPerEntranceTest.Value.ToString());

            if (updateModel.MaxStudentsPerEntranceTest.HasValue)
                await UpsertSystemConfig(ConfigNames.MaxStudentsInTest, SystemConfigType.UnsignedInt,
                    updateModel.MaxStudentsPerEntranceTest.Value.ToString());

            if (updateModel.AllowEntranceTestRegistering.HasValue)
                await UpsertSystemConfig(ConfigNames.AllowEntranceTestRegistering, SystemConfigType.Text,
                    updateModel.AllowEntranceTestRegistering.Value ? "true" : "false");

            if (updateModel.TestFee.HasValue)
                await UpsertSystemConfig(ConfigNames.TestFee, SystemConfigType.UnsignedInt,
                    updateModel.TestFee.Value.ToString());

            if (updateModel.TheoryPercentage.HasValue)
                await UpsertSystemConfig(ConfigNames.TheoryPercentage, SystemConfigType.UnsignedInt,
                    updateModel.TheoryPercentage.Value.ToString());

            if (updateModel.PracticePercentage.HasValue)
                await UpsertSystemConfig(ConfigNames.PracticePercentage, SystemConfigType.UnsignedInt,
                    updateModel.PracticePercentage.Value.ToString());
        });
        await _serviceFactory.RedisCacheService.DeleteByPatternAsync("SystemConfig");
    }

    public async Task UpdateTuitionSystemConfig(UpdateTuitionSystemConfigModel updateModel)
    {
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            if (updateModel.DeadlineForPayTuition.HasValue)
                await UpsertSystemConfig(ConfigNames.TuitionPaymentDeadline, SystemConfigType.UnsignedInt,
                    updateModel.DeadlineForPayTuition.Value.ToString());

            if (updateModel.SlotTrial.HasValue)
                await UpsertSystemConfig(ConfigNames.NumTrialSlot, SystemConfigType.UnsignedInt,
                    updateModel.SlotTrial.Value.ToString());

            if (updateModel.TaxRates.HasValue)
                await UpsertSystemConfig(ConfigNames.TaxRates, SystemConfigType.Text,
                    updateModel.TaxRates.Value.ToString());
        });
        await _serviceFactory.RedisCacheService.DeleteByPatternAsync("SystemConfig");
    }

    public async Task UpdateRefundSystemConfig(UpdateRefundSystemConfigModel updateModel)
    {
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            if (updateModel.ReasonRefundTuition != null)
            {
                var jsonReasons = JsonConvert.SerializeObject(updateModel.ReasonRefundTuition);

                await UpsertSystemConfig(ConfigNames.ReasonForRefund, SystemConfigType.Text, jsonReasons);
            }
        });

        await _serviceFactory.RedisCacheService.DeleteByPatternAsync("SystemConfig");
    }

    public async Task UpdateSchedulerSystemConfig(UpdateSchedulerSystemConfigModel updateModel)
    {
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            if (updateModel.DeadlineAttendance.HasValue)
                await UpsertSystemConfig(ConfigNames.AttendanceDeadline, SystemConfigType.UnsignedInt,
                    updateModel.DeadlineAttendance.Value.ToString());

            if (updateModel.ReasonCancelSlot != null)
            {
                var jsonReasons = JsonConvert.SerializeObject(updateModel.ReasonCancelSlot);

                await UpsertSystemConfig(ConfigNames.ReasonForCancelSlot, SystemConfigType.Text, jsonReasons);
            }

            if (updateModel.MaxAbsenceRate.HasValue)
                await UpsertSystemConfig(ConfigNames.MaxAbsenceRate, SystemConfigType.UnsignedInt,
                    updateModel.MaxAbsenceRate.Value.ToString());
        });
        await _serviceFactory.RedisCacheService.DeleteByPatternAsync("SystemConfig");
    }


    public async Task<List<SystemConfigModel>> GetAllSurveyConfigs()
    {
        var surveyConfigs = await _unitOfWork.SystemConfigRepository.FindProjectedAsync<SystemConfigModel>(
            c =>
                _surveyConfigNames.Contains(c.ConfigName),
            false);

        return surveyConfigs;
    }

    public async Task<List<SystemConfigModel>> GetEntranceTestConfigs(params List<string> configNames)
    {
        return await _unitOfWork.SystemConfigRepository.FindProjectedAsync<SystemConfigModel>(
            c =>
                configNames.Count == 0
                    ? _entranceTestConfigNames.Contains(c.ConfigName)
                    : configNames.Contains(c.ConfigName),
            false);
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

    public async Task UpdateClassSystemConfig(UpdateClassSystemConfigModel updateModel)
    {
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            if (updateModel.MaximumClassSize.HasValue)
                await UpsertSystemConfig(ConfigNames.MaximumStudents, SystemConfigType.UnsignedInt,
                    updateModel.MaximumClassSize.Value.ToString());

            if (updateModel.MinimumClassSize.HasValue)
                await UpsertSystemConfig(ConfigNames.MinimumStudents, SystemConfigType.UnsignedInt,
                    updateModel.MinimumClassSize.Value.ToString());

            if (updateModel.DeadlineChangingClass.HasValue)
                await UpsertSystemConfig(ConfigNames.ChangingClassDeadline, SystemConfigType.Boolean,
                    updateModel.DeadlineChangingClass.Value.ToString());
            if (updateModel.AllowSkippingLevel.HasValue)
                await UpsertSystemConfig(ConfigNames.AllowSkippingLevel, SystemConfigType.UnsignedInt,
                    updateModel.AllowSkippingLevel.Value ? "true" : "false");
        });
        await _serviceFactory.RedisCacheService.DeleteByPatternAsync("SystemConfig");
    }
}