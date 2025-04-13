using PhotonPiano.BusinessLogic.BusinessModel.Level;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ILevelService
{
    Task<List<LevelModel>> GetAllLevelsAsync();
    Task<List<LevelModel>> GetCachedAllLevelsAsync();
    Task<Guid> GetLevelIdFromScores(decimal theoreticalScore, decimal practicalScore);
    Task<LevelDetailsModel> GetLevelDetailsAsync(Guid levelId);
}