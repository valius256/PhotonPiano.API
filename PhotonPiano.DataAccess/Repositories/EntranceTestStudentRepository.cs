using Microsoft.EntityFrameworkCore;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.Repositories;

public class EntranceTestStudentRepository : GenericRepository<EntranceTestStudent>, IEntranceTestStudentRepository
{
    private readonly ApplicationDbContext _context;

    public EntranceTestStudentRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<EntranceTestStudent>> GetEntranceTestStudentsWithResults(Guid entranceTestId)
    {
        return await _context.EntranceTestStudents.Where(x => x.EntranceTestId == entranceTestId)
            .Include(x => x.EntranceTestResults)
            .ToListAsync();
    }
}