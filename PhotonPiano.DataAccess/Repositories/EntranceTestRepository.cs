using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.Repositories
{
    public class EntranceTestRepository : GenericRepository<EntranceTest>, IEntranceTestRepository
    {
        private readonly ApplicationDbContext _context;
        public EntranceTestRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
