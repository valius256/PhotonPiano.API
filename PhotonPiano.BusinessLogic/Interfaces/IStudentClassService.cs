using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;

namespace PhotonPiano.BusinessLogic.Interfaces
{
    public interface IStudentClassService
    {
        Task<List<StudentClassModel>> CreateStudentClass(CreateStudentClassModel createStudentClassModel, string accountFirebaseId);

        Task ChangeClassOfStudent(ChangeClassModel changeClassModel, AccountModel account);

        Task DeleteStudentClass(string studentId, Guid classId, bool isExpelled, string accountFirebaseId);
    }
}
