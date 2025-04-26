using Microsoft.EntityFrameworkCore;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.Repositories;

public class EntranceTestRepository : GenericRepository<EntranceTest>, IEntranceTestRepository
{
    private readonly ApplicationDbContext _context;

    public EntranceTestRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<EntranceTest?> GetEntranceTestWithStudentsAsync(Guid id)
    {
        return await _context.EntranceTests
            .Include(e => e.EntranceTestStudents)
            .SingleOrDefaultAsync(e => e.Id == id);
    }
}