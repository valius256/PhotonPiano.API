using PhotonPiano.BusinessLogic.BusinessModel.Account;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IStudentClassScoreService
{
    Task PublishScore(Guid classId, AccountModel account);
}