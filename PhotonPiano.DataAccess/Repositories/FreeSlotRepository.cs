
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.Repositories
{
    public class FreeSlotRepository : GenericRepository<FreeSlot>, IFreeSlotRepository
    {
        public FreeSlotRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
