using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.Repositories;

public class PianoQuestionRepository : GenericRepository<PianoQuestion>, IPianoQuestionRepository
{
    public PianoQuestionRepository(ApplicationDbContext context) : base(context)
    {
    }
}