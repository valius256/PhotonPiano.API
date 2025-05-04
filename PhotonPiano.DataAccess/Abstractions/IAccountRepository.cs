using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.Abstractions;

public interface IAccountRepository : IGenericRepository<Account>
{
    Task<List<Account>> GetStudentsWithEntranceTestStudents(StudentStatus studentStatus, params List<string> accountIds);
}