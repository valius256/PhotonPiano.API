using PhotonPiano.BusinessLogic.BusinessModel.Level;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services;

public class LevelService : ILevelService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IServiceFactory _serviceFactory;

    private readonly string _cacheKey = "levels";

    public LevelService(IUnitOfWork unitOfWork, IServiceFactory serviceFactory)
    {
        _unitOfWork = unitOfWork;
        _serviceFactory = serviceFactory;
    }

    public async Task<List<LevelModel>> GetAllLevelsAsync()
    {
        var levels = await _unitOfWork.LevelRepository.FindProjectedAsync<LevelModel>(hasTrackings: false);

        // Find the root level (the one that no other level references as its NextLevel)
        var rootLevel = levels.FirstOrDefault(l => levels.All(x => x.NextLevelId != l.Id));

        var sortedLevels = new List<LevelModel>();

        while (rootLevel is not null)
        {
            sortedLevels.Add(rootLevel);
            rootLevel = levels.FirstOrDefault(l => l.Id == rootLevel.NextLevelId);
        }

        return sortedLevels;
    }

    public async Task<List<LevelModel>> GetCachedAllLevelsAsync()
    {
        var cachedLevels = await _serviceFactory.RedisCacheService.GetAsync<List<LevelModel>>(_cacheKey);

        if (cachedLevels is not null && cachedLevels.Count > 0)
        {
            return cachedLevels;
        }

        var levels = await GetAllLevelsAsync();

        await _serviceFactory.RedisCacheService.SaveAsync(key: _cacheKey, value: levels, expiry: TimeSpan.FromDays(1));

        return levels;
    }

    public async Task<Guid> GetLevelIdFromScores(decimal theoreticalScore, decimal practicalScore)
    {
        var level = await _unitOfWork.LevelRepository.GetLevelByScoresAsync(theoreticalScore, practicalScore);

        if (level is null)
        {
            throw new BadRequestException("Invalid score");
        }

        return level.Id;
    }

    public async Task<LevelDetailsModel> GetLevelDetailsAsync(Guid levelId)
    {
        var level = await _unitOfWork.LevelRepository.FindSingleProjectedAsync<LevelDetailsModel>(l => l.Id == levelId,
            hasTrackings: false);

        if (level is null)
        {
            throw new NotFoundException("Level not found");
        }

        return level;
    }
}