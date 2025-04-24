using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.StudentScore;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IStudentClassScoreService
{
    Task PublishScore(Guid classId, AccountModel account);
    Task<ClassScoreViewModel> GetClassScoresWithCriteria(Guid classId);

    Task<StudentDetailedScoreViewModel> GetStudentDetailedScores(Guid studentClassId);
}