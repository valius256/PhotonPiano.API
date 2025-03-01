using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.Repositories;

public class EntranceTestResultRepository : GenericRepository<EntranceTestResult>, IEntranceTestResultRepository
{
    public EntranceTestResultRepository(ApplicationDbContext context) : base(context)
    {
    }
}