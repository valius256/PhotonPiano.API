using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Level;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ILevelService
{
    Task<List<LevelModel>> GetAllLevelsAsync();
    Task<List<LevelModel>> GetCachedAllLevelsAsync();
    Task<Guid> GetLevelIdFromScores(decimal theoreticalScore, decimal practicalScore);
    Task<LevelModel> CreateLevelAsync(CreateLevelModel createModel, AccountModel currentAccount);
    Task UpdateLevelAsync(Guid id, UpdateLevelModel updateModel, AccountModel currentAccount);
    Task DeleteLevelAsync(Guid id, Guid fallBackLevelId);
    Task<LevelDetailsModel> GetLevelDetailsAsync(Guid levelId);
    Task UpdateLevelMinimumGpaAsync(Guid id, UpdateLevelMinimumGpaModel model);

    Task ChangeLevelOrder(UpdateLevelOrderModel updateLevelOrderModel);
}