using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;

namespace PhotonPiano.BusinessLogic.Interfaces
{
    public interface IStudentClassService
    {
        Task<StudentClassModel> CreateStudentClass(CreateStudentClassModel createStudentClassModel, string accountFirebaseId);

        Task ChangeClassOfStudent(ChangeClassModel changeClassModel, string accountFirebaseId);

        Task DeleteStudentClass(string studentId, Guid classId, bool isExpelled, string accountFirebaseId);
        
        Task<byte[]> GenerateGradeTemplate(Guid classId);
        Task<bool> ImportScores(Guid classId, Stream excelFileStream, AccountModel account);
    }
}
