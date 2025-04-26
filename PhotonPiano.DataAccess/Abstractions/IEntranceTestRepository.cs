using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.Abstractions;

public interface IEntranceTestRepository : IGenericRepository<EntranceTest>
{
    Task<EntranceTest?> GetEntranceTestWithStudentsAsync(Guid id);
}