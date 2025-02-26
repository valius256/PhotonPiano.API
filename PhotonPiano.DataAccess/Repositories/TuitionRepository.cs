using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.Repositories;

public class TuitionRepository : GenericRepository<Tuition>, ITuitionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TuitionRepository(ApplicationDbContext context) : base(context)
    {
        _dbContext = context;
    }
}