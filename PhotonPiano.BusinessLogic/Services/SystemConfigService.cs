using Microsoft.EntityFrameworkCore;
using PhotonPiano.BusinessLogic.BusinessModel.SystemConfig;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services;

public class SystemConfigService : ISystemConfigService
{
    private readonly IUnitOfWork _unitOfWork;

    public SystemConfigService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<SystemConfig> GetConfig(string name)
    {
        var config = await _unitOfWork.SystemConfigRepository.FindFirstAsync(c => c.ConfigName == name);
        if (config is null) throw new NotFoundException("Config not found");
        return config;
    }

    public async Task SetConfigValue(UpdateSystemConfigModel updateSystemConfigModel)
    {
        var config = await GetConfig(updateSystemConfigModel.ConfigName);
        config.ConfigValue = updateSystemConfigModel.ConfigValue;

        await _unitOfWork.SystemConfigRepository.UpdateAsync(config);
        await _unitOfWork.SaveChangesAsync();
    }

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
}