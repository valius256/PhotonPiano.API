
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.DataAccess.Repositories
{
    public class StudentClassScoreRepository : GenericRepository<StudentClassScore>, IStudentClassScoreRepository
    {
        public StudentClassScoreRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
