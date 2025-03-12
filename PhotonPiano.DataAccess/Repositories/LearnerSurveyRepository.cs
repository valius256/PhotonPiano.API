using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.Repositories;

public class LearnerSurveyRepository : GenericRepository<LearnerSurvey>, ILearnerSurveyRepository
{
    public LearnerSurveyRepository(ApplicationDbContext context) : base(context)
    {
    }
}