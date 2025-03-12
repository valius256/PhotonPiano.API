using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.Repositories;

public class SurveyQuestionRepository : GenericRepository<SurveyQuestion>, ISurveyQuestionRepository
{
    public SurveyQuestionRepository(ApplicationDbContext context) : base(context)
    {
    }
}