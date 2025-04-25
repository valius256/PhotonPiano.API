using Mapster;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Level;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
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


    private async Task<List<Level>> GetSortedAllLevels()
    {
        var levels = await _unitOfWork.LevelRepository.GetAllAsync(hasTrackings: false);

        var levelDict = levels.ToDictionary(l => l.Id);
        var nextIdSet = new HashSet<Guid>(levels
            .Where(l => l.NextLevelId.HasValue)
            .Select(l => l.NextLevelId!.Value));

        var rootLevel = levels.FirstOrDefault(l => !nextIdSet.Contains(l.Id));

        if (rootLevel is null)
        {
            throw new BadRequestException("No root level found. The level chain might be broken or circular.");
        }

        var sortedLevels = new List<Level>();
        var visited = new HashSet<Guid>();

        var current = rootLevel;

        while (current != null)
        {
            if (!visited.Add(current.Id))
            {
                throw new BadRequestException("Circular reference detected in level chain.");
            }

            sortedLevels.Add(current);

            current = current.NextLevelId is { } nextId && levelDict.TryGetValue(nextId, out var next)
                ? next
                : null;
        }

        return sortedLevels;
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

    private async Task UpdateLevels(List<Level> levels, Level newLevel, Guid? nextLevelId)
    {
        if (levels.Count == 0)
        {
            return;
        }

        if (levels.Count == 1)
        {
            var lastLevel = levels.LastOrDefault(l => l.NextLevelId == null);

            await _unitOfWork.LevelRepository.ExecuteUpdateAsync(l => l.Id == lastLevel!.Id,
                setter => setter.SetProperty(l => l.NextLevelId, newLevel.Id));

            await _unitOfWork.LevelRepository.ExecuteUpdateAsync(l => l.Id == newLevel.Id,
                setter => setter.SetProperty(l => l.NextLevelId, nextLevelId));

            return;
        }

        var nextLevel = levels.FirstOrDefault(l => l.Id == nextLevelId);

        if (nextLevel is null)
        {
            throw new BadRequestException("Invalid next level");
        }

        var prevLevel = levels.FirstOrDefault(l => l.NextLevelId == nextLevel.Id);

        if (prevLevel is null)
        {
            throw new BadRequestException("Invalid next level");
        }

        if (newLevel.MinimumTheoreticalScore <= prevLevel.MinimumTheoreticalScore
            || newLevel.MinimumTheoreticalScore >= nextLevel.MinimumTheoreticalScore)
        {
            throw new BadRequestException("Theory score must between the minimum and the maximum theoretical score");
        }

        if (newLevel.MinimumPracticalScore <= prevLevel.MinimumPracticalScore
            || newLevel.MinimumPracticalScore >= nextLevel.MinimumPracticalScore)
        {
            throw new BadRequestException("Theory score must between the minimum and the maximum practical score");
        }

        await _unitOfWork.LevelRepository.ExecuteUpdateAsync(l => l.Id == prevLevel.Id,
            setter => setter.SetProperty(l => l.NextLevelId, newLevel.Id));

        await _unitOfWork.LevelRepository.ExecuteUpdateAsync(l => l.Id == newLevel.Id,
            setter => setter.SetProperty(l => l.NextLevelId, nextLevelId));
    }

    public async Task<LevelModel> CreateLevelAsync(CreateLevelModel createModel, AccountModel currentAccount)
    {
        var nextLevelId = createModel.NextLevelId;
        var levels = await GetSortedAllLevels();
        var newLevel = createModel.Adapt<Level>();

        newLevel.NextLevelId = null;
        newLevel.Id = Guid.NewGuid();

        await _unitOfWork.LevelRepository.AddAsync(newLevel);
        await _unitOfWork.SaveChangesAsync();

        await UpdateLevels(levels, newLevel, nextLevelId);

        await _serviceFactory.RedisCacheService.DeleteAsync(_cacheKey);

        return newLevel.Adapt<LevelModel>();
    }

    public async Task UpdateLevelAsync(Guid id, UpdateLevelModel updateModel, AccountModel currentAccount)
    {
        var level = await _unitOfWork.LevelRepository.FindSingleAsync(l => l.Id == id);

        if (level is null)
        {
            throw new NotFoundException("Level not found.");
        }

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            updateModel.Adapt(level);

            var levels = await GetSortedAllLevels();

            await UpdateLevels(levels, level, level.NextLevelId);

            level.UpdatedAt = DateTime.UtcNow.AddHours(7);
        });

        await _serviceFactory.RedisCacheService.DeleteAsync(_cacheKey);
    }

    private void RemoveLevel(List<Level> levels, Level levelToRemove)
    {
        if (levels.Count <= 1)
        {
            return;
        }

        var removeIndex = levels.IndexOf(levelToRemove);

        if (removeIndex == 0)
        {
            return;
        }

        if (removeIndex == levels.Count - 1)
        {
            var penultimateLevel = levels[^2];
            penultimateLevel.NextLevelId = null;
            return;
        }

        var prevLevel = levels[removeIndex - 1];
        var nextLevel = levels[removeIndex + 1];

        prevLevel.NextLevelId = nextLevel.Id;
    }

    public async Task DeleteLevelAsync(Guid id)
    {
        var level = await _unitOfWork.LevelRepository.FindSingleAsync(l => l.Id == id);

        if (level is null)
        {
            throw new NotFoundException("Level not found.");
        }

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var levels = await GetSortedAllLevels();

            RemoveLevel(levels, level);

            level.RecordStatus = RecordStatus.IsDeleted;
            level.DeletedAt = DateTime.UtcNow.AddHours(7);
        });

        await _serviceFactory.RedisCacheService.DeleteAsync(_cacheKey);
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