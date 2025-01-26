using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.Repositories
{
    public class SlotStudentRepository : GenericRepository<SlotStudent>, ISlotStudentRepository
    {
        public SlotStudentRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
