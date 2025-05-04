using Microsoft.EntityFrameworkCore;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.Repositories;

public class AccountRepository : GenericRepository<Account>, IAccountRepository
{
    private readonly ApplicationDbContext _context;

    public AccountRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Account>> GetStudentsWithEntranceTestStudents(StudentStatus studentStatus,
        params List<string> accountIds)
    {
        var students = await _context.Accounts
            .Where(a => accountIds.Contains(a.AccountFirebaseId) && a.Role == Role.Student &&
                        a.StudentStatus == studentStatus)
            .Include(a => a.EntranceTestStudents)
            .ToListAsync();

        return students;
    }
}