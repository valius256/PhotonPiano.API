using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.Repositories;

public class LearnerAnswerRepository : GenericRepository<LearnerAnswer>, ILearnerAnswerRepository
{
    public LearnerAnswerRepository(ApplicationDbContext context) : base(context)
    {
    }
}