using Microsoft.EntityFrameworkCore;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.Repositories;

public class PianoSurveyRepository : GenericRepository<PianoSurvey>, IPianoSurveyRepository
{
    private readonly ApplicationDbContext _context;

    public PianoSurveyRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<PianoSurvey?> GetPianoSurveyWithQuestionsAsync(Guid id)
    {
        return await _context.PianoSurveys
            .Include(ps => ps.Questions)
            .SingleOrDefaultAsync(ps => ps.Id == id);
    }
}