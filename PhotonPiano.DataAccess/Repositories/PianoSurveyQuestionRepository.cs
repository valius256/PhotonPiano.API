using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.Repositories;

public class PianoSurveyQuestionRepository : GenericRepository<PianoSurveyQuestion>, IPianoSurveyQuestionRepository
{
    public PianoSurveyQuestionRepository(ApplicationDbContext context) : base(context)
    {
    }
}