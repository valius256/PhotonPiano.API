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
}