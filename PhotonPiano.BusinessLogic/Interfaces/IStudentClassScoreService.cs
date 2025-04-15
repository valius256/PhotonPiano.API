using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IStudentClassScoreService
{
    Task PublishScore(Guid classId, AccountModel account);
    Task<ClassScoreViewModel> GetClassScoresWithCriteria(Guid classId);
    Task UnpublishScore(Guid classId, AccountModel account);
}