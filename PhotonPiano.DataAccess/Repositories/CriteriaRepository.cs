using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.Repositories;

public class CriteriaRepository : GenericRepository<Criteria>, ICriteriaRepository
{
    private readonly ApplicationDbContext _context;

    public CriteriaRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}