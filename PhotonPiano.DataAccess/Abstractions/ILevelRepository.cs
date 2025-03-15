using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.Abstractions;

public interface ILevelRepository : IGenericRepository<Level>
{
    Task<Level?> GetLevelByScoresAsync(decimal theoreticalScore, decimal practicalScore);
}