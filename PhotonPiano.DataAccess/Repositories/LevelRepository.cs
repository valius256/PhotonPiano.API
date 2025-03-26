using Microsoft.EntityFrameworkCore;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.Repositories
{
    public class LevelRepository : GenericRepository<Level>, ILevelRepository
    {
        private readonly ApplicationDbContext _context;

        public LevelRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Level?> GetLevelByScoresAsync(decimal theoreticalScore, decimal practicalScore)
        {
            return await _context.Levels
                .Where(l => theoreticalScore >= l.MinimumTheoreticalScore && practicalScore >= l.MinimumPracticalScore)
                .OrderByDescending(l => l.MinimumPracticalScore) // Prioritize higher practical score
                .ThenByDescending(l => l.MinimumTheoreticalScore) // Secondary priority: higher theoretical score
                .FirstOrDefaultAsync();
        }
    }
}
