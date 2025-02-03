using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.Repositories;

public class DayOffRepository : GenericRepository<DayOff>, IDayOffRepository
{
    public DayOffRepository(ApplicationDbContext context) : base(context)
    {
    }
}