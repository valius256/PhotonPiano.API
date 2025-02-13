using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.Repositories;

public class ApplicationRepository : GenericRepository<Application>, IApplicationRepository
{
    public ApplicationRepository(ApplicationDbContext context) : base(context)
    {
    }
}