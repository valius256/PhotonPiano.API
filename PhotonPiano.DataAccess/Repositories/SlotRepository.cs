using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.Repositories;

public class SlotRepository : GenericRepository<Slot>, ISlotRepository
{
    private readonly ApplicationDbContext _context;

    public SlotRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}