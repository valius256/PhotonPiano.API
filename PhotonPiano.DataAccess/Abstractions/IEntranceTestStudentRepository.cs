using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.Abstractions;

public interface IEntranceTestStudentRepository : IGenericRepository<EntranceTestStudent>
{
    Task<List<EntranceTestStudent>> GetEntranceTestStudentsWithResults(Guid entranceTestId);
}