using Mapster;
using Microsoft.EntityFrameworkCore;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.Level;
using PhotonPiano.BusinessLogic.Extensions;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Exceptions;
using PhotonPiano.Shared.Utils;

namespace PhotonPiano.BusinessLogic.Services;

public class LevelService : ILevelService
{
    private readonly string _cacheKey = "levels";

    private readonly IServiceFactory _serviceFactory;
    private readonly IUnitOfWork _unitOfWork;

    public LevelService(IUnitOfWork unitOfWork, IServiceFactory serviceFactory)
    {
        _unitOfWork = unitOfWork;
        _serviceFactory = serviceFactory;
    }

    public async Task<List<LevelModel>> GetAllLevelsAsync()
    {
        var levels =
            await _unitOfWork.LevelRepository.FindProjectedAsync<LevelModel>(hasTrackings: false,
                ignoreQueryFilters: false);

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

        if (cachedLevels is not null && cachedLevels.Count > 0) return cachedLevels;

        var levels = await GetAllLevelsAsync();

        await _serviceFactory.RedisCacheService.SaveAsync(_cacheKey, levels, TimeSpan.FromDays(1));

        return levels;
    }

    public async Task<Guid> GetLevelIdFromScores(decimal theoreticalScore, decimal practicalScore)
    {
        var level = await _unitOfWork.LevelRepository.GetLevelByScoresAsync(theoreticalScore, practicalScore);

        if (level is null) throw new BadRequestException("Invalid score");

        return level.Id;
    }

    //private async Task UpdateLevels(List<Level> levels, Level newLevel, Guid? nextLevelId)
    //{
    //    if (levels.Count == 0)
    //    {
    //        return;
    //    }

    //    if (levels.Count == 1)
    //    {
    //        var lastLevel = levels.LastOrDefault(l => l.NextLevelId == null);

    //        await _unitOfWork.LevelRepository.ExecuteUpdateAsync(l => l.Id == lastLevel!.Id,
    //            setter => setter.SetProperty(l => l.NextLevelId, newLevel.Id));

    //        await _unitOfWork.LevelRepository.ExecuteUpdateAsync(l => l.Id == newLevel.Id,
    //            setter => setter.SetProperty(l => l.NextLevelId, nextLevelId));

    //        return;
    //    }

    //    var nextLevel = levels.FirstOrDefault(l => l.Id == nextLevelId);

    //    if (nextLevel is null)
    //    {
    //        throw new BadRequestException("Invalid next level");
    //    }

    //    var prevLevel = levels.FirstOrDefault(l => l.NextLevelId == nextLevel.Id);

    //    if (prevLevel is null)
    //    {
    //        throw new BadRequestException("Invalid next level");
    //    }

    //    if (newLevel.MinimumTheoreticalScore <= prevLevel.MinimumTheoreticalScore
    //        || newLevel.MinimumTheoreticalScore >= nextLevel.MinimumTheoreticalScore)
    //    {
    //        throw new BadRequestException("Theory score must between the minimum and the maximum theoretical score");
    //    }

    //    if (newLevel.MinimumPracticalScore <= prevLevel.MinimumPracticalScore
    //        || newLevel.MinimumPracticalScore >= nextLevel.MinimumPracticalScore)
    //    {
    //        throw new BadRequestException("Theory score must between the minimum and the maximum practical score");
    //    }

    //    await _unitOfWork.LevelRepository.ExecuteUpdateAsync(l => l.Id == prevLevel.Id,
    //        setter => setter.SetProperty(l => l.NextLevelId, newLevel.Id));

    //    await _unitOfWork.LevelRepository.ExecuteUpdateAsync(l => l.Id == newLevel.Id,
    //        setter => setter.SetProperty(l => l.NextLevelId, nextLevelId));
    //}

    public async Task UpdateLevelMinimumGpaAsync(Guid id, UpdateLevelMinimumGpaModel model)
    {
        if (model.MinimumGpa < 0 || model.MinimumGpa > 10)
            throw new BadRequestException("Minimum GPA must be between 0 and 10");

        var level = await _unitOfWork.LevelRepository.GetByIdAsync(id);
        if (level is null || level.RecordStatus == RecordStatus.IsDeleted)
            throw new NotFoundException($"Level with ID {level!.Id} not found");

        level.MinimumGPA = model.MinimumGpa;
        level.UpdatedAt = DateTime.UtcNow.AddHours(7);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<LevelModel> CreateLevelAsync(CreateLevelModel createModel, AccountModel currentAccount)
    {
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var newLevel = createModel.Adapt<Level>();
            newLevel.Id = Guid.NewGuid();

            // Step 1: Save the new level with null NextLevelId
            newLevel.NextLevelId = null;
            await _unitOfWork.LevelRepository.AddAsync(newLevel);
            await _unitOfWork.SaveChangesAsync(); // Avoid circular dependency during add

            // Step 2: Wire up pointers after the level exists
            if (createModel.NextLevelId.HasValue)
            {
                var nextLevel = await _unitOfWork.LevelRepository.FindSingleAsync(l => l.Id == createModel.NextLevelId);
                if (nextLevel is null)
                    throw new NotFoundException("Next level not found");

                var prevLevel = await _unitOfWork.LevelRepository.FindSingleAsync(l => l.NextLevelId == nextLevel.Id);
                if (prevLevel != null)
                    prevLevel.NextLevelId = newLevel.Id;

                newLevel.NextLevelId = nextLevel.Id;
            }
            else
            {
                var prevLevel = await _unitOfWork.LevelRepository.FindSingleAsync(l => l.NextLevelId == null);
                if (prevLevel != null)
                    prevLevel.NextLevelId = newLevel.Id;

                newLevel.NextLevelId = null;
            }

            // Step 3: Save wiring updates
            await _unitOfWork.SaveChangesAsync();
            await _serviceFactory.RedisCacheService.DeleteAsync(_cacheKey);

            return newLevel.Adapt<LevelModel>();
        });
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

            // await UpdateLevels(levels, level, level.NextLevelId);

            level.UpdatedAt = DateTime.UtcNow.AddHours(7);
        });

        await _serviceFactory.RedisCacheService.DeleteAsync(_cacheKey);
    }

    //private void RemoveLevel(List<Level> levels, Level levelToRemove)
    //{
    //    if (levels.Count <= 1)
    //    {
    //        return;
    //    }

    //    var removeIndex = levels.IndexOf(levelToRemove);

    //    if (removeIndex == 0)
    //    {
    //        return;
    //    }

    //    if (removeIndex == levels.Count - 1)
    //    {
    //        var penultimateLevel = levels[^2];
    //        penultimateLevel.NextLevelId = null;
    //        return;
    //    }

    //    var prevLevel = levels[removeIndex - 1];
    //    var nextLevel = levels[removeIndex + 1];

    //    prevLevel.NextLevelId = nextLevel.Id;
    //}

    public async Task DeleteLevelAsync(Guid id, Guid fallBackLevelId)
    {
        var level = await _unitOfWork.LevelRepository.FindSingleAsync(l => l.Id == id);

        if (level is null) throw new NotFoundException("Level not found.");

        var fallbackLevel = await _unitOfWork.LevelRepository.FindSingleAsync(l => l.Id == fallBackLevelId);

        if (fallbackLevel is null) throw new NotFoundException("Fallback level not found.");

        var affectedAccounts = await _unitOfWork.AccountRepository.FindAsync(a => a.LevelId == level.Id);
        var affectedClasses =
            await _unitOfWork.ClassRepository.FindAsync(a => a.LevelId == level.Id && a.Status != ClassStatus.Finished);

        var nextLevel = await _unitOfWork.LevelRepository.FindSingleAsync(l => l.Id == level.NextLevelId);
        var prevLevel = await _unitOfWork.LevelRepository.FindSingleAsync(l => l.NextLevelId == level.Id);

        if (prevLevel is not null)
        {
            if (nextLevel is not null)
                prevLevel.NextLevelId = nextLevel.Id;
            else
                prevLevel.NextLevelId = null;
        }

        foreach (var classInfo in affectedClasses) classInfo.LevelId = fallBackLevelId;

        foreach (var account in affectedAccounts) account.LevelId = fallBackLevelId;

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            //var levels = await GetSortedAllLevels();
            level.RecordStatus = RecordStatus.IsDeleted;
            level.NextLevelId = null;
            level.DeletedAt = DateTime.UtcNow.AddHours(7);

            //RemoveLevel(levels, level);
            await _unitOfWork.AccountRepository.UpdateRangeAsync(affectedAccounts);
            await _unitOfWork.ClassRepository.UpdateRangeAsync(affectedClasses);
        });

        await _serviceFactory.RedisCacheService.DeleteAsync(_cacheKey);
    }

    public async Task<LevelDetailsModel> GetLevelDetailsAsync(Guid levelId)
    {
        var level = await _unitOfWork.LevelRepository.FindSingleProjectedAsync<LevelDetailsModel>(l => l.Id == levelId,
            false);

        if (level is null) throw new NotFoundException("Level not found");


        // khong load bang mapping vi issue take time so long
        await LoadAndSetClassTimesAsync(level.Classes.ToList());


        return level;
    }

    private async Task LoadAndSetClassTimesAsync(List<ClassModel> classModels)
    {
        var classIds = classModels
            .Where(c => c is { IsPublic: true, Status: ClassStatus.NotStarted })
            .Select(c => c.Id)
            .ToList();
        // && c.Status == ClassStatus.NotStarted
        var classWithSlots = await _unitOfWork.ClassRepository
            .FindProjectedAsync<Class>(c => classIds.Contains(c.Id), false);

        var capacity = await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.MaximumStudents);

        foreach (var classModel in classModels)
        {
            var matchedClass = classWithSlots.FirstOrDefault(c => c.Id == classModel.Id);
            if (matchedClass != null)
            {
                classModel.ClassTime = DateExtensions.FormatTime(matchedClass.Slots);
                classModel.ClassDays = DateExtensions.FormatDays(matchedClass.Slots);
                classModel.TotalSlots = matchedClass.Slots.Count;
                classModel.Capacity = int.Parse(capacity?.ConfigValue ?? "1");
            }
        }
    }

    public async Task ChangeLevelOrder(UpdateLevelOrderModel updateLevelOrderModel)
    {
        // Validation: all levels are included
        var allLevelIds = await _unitOfWork.LevelRepository.Entities.Select(l => l.Id).ToListAsync();
        var providedIds = updateLevelOrderModel.LevelOrders.Select(lo => lo.Id).ToList();

        if (allLevelIds.Except(providedIds).Any() || providedIds.Except(allLevelIds).Any())
            throw new BadRequestException("Provided levels do not match existing levels exactly.");

        // Validation: no duplicate NextLevelId targets
        var nextLevelTargets = updateLevelOrderModel.LevelOrders.Where(lo => lo.NextLevelId != null)
            .Select(lo => lo.NextLevelId!.Value).ToList();
        if (nextLevelTargets.GroupBy(x => x).Any(g => g.Count() > 1))
            throw new BadRequestException("Multiple levels cannot point to the same NextLevelId.");

        // Validation: no cycles
        if (HasCycle(updateLevelOrderModel.LevelOrders))
            throw new BadRequestException("Level chain has a cycle.");

        // All validations passed → apply updates
        var levels = await _unitOfWork.LevelRepository.Entities.ToDictionaryAsync(l => l.Id);

        foreach (var orderModel in updateLevelOrderModel.LevelOrders)
            if (levels.TryGetValue(orderModel.Id, out var level))
                level.NextLevelId = orderModel.NextLevelId;

        await _unitOfWork.SaveChangesAsync();
        await _serviceFactory.RedisCacheService.DeleteAsync(_cacheKey);
    }

    public async Task<bool> IsFirstLevelAsync(Guid levelId)
    {
        var levelPointingTo =
            await _unitOfWork.LevelRepository.FindFirstAsync(l => l.NextLevelId == levelId, false);

        return levelPointingTo is null;
    }


    private async Task<List<Level>> GetSortedAllLevels()
    {
        var levels = await _unitOfWork.LevelRepository.GetAllAsync(false);

        var levelDict = levels.ToDictionary(l => l.Id);
        var nextIdSet = new HashSet<Guid>(levels
            .Where(l => l.NextLevelId.HasValue)
            .Select(l => l.NextLevelId!.Value));

        var rootLevel = levels.FirstOrDefault(l => !nextIdSet.Contains(l.Id));

        if (rootLevel is null)
            throw new BadRequestException("No root level found. The level chain might be broken or circular.");

        var sortedLevels = new List<Level>();
        var visited = new HashSet<Guid>();

        var current = rootLevel;

        while (current != null)
        {
            if (!visited.Add(current.Id)) throw new BadRequestException("Circular reference detected in level chain.");

            sortedLevels.Add(current);

            current = current.NextLevelId is { } nextId && levelDict.TryGetValue(nextId, out var next)
                ? next
                : null;
        }

        return sortedLevels;
    }

    private bool HasCycle(List<LevelOrderModel> levels)
    {
        var visited = new HashSet<Guid>();
        var nextMap = levels.ToDictionary(l => l.Id, l => l.NextLevelId);

        // Find the starting node (not pointed to by any other level)
        var allIds = new HashSet<Guid>(levels.Select(l => l.Id));
        var pointedTo = new HashSet<Guid>(levels.Where(l => l.NextLevelId.HasValue).Select(l => l.NextLevelId!.Value));
        var startNodes = allIds.Except(pointedTo).ToList();

        if (startNodes.Count != 1)
            return true; // Either disconnected or multiple heads

        Guid? current = startNodes.First();

        while (current.HasValue)
        {
            if (visited.Contains(current.Value))
                return true; // Cycle detected

            visited.Add(current.Value);

            // Move to the next level (if exists)
            current = nextMap.TryGetValue(current.Value, out var next) ? next : null;
        }

        // Ensure all levels are connected
        return visited.Count != levels.Count;
    }
}