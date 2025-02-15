using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.Repositories;

public class AccountNotificationRepository : GenericRepository<AccountNotification>, IAccountNotificationRepository
{
    private readonly ApplicationDbContext _context;

    public AccountNotificationRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}