

using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.Repositories
{
    public class SystemConfigRepository : GenericRepository<SystemConfig>, ISystemConfigRepository
    {
        private readonly ApplicationDbContext _context;
        public SystemConfigRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
