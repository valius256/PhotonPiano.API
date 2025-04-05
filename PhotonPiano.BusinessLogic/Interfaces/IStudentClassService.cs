using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;

namespace PhotonPiano.BusinessLogic.Interfaces
{
    public interface IStudentClassService
    {
        Task<List<StudentClassModel>> CreateStudentClass(CreateStudentClassModel createStudentClassModel, string accountFirebaseId);

        Task ChangeClassOfStudent(ChangeClassModel changeClassModel, AccountModel account);

        Task DeleteStudentClass(string studentId, Guid classId, bool isExpelled, string accountFirebaseId);
        
        Task<byte[]> GenerateGradeTemplate(Guid classId);
        Task<bool> ImportScores(Guid classId, Stream excelFileStream, AccountModel account);

        Task<bool> UpdateStudentStatusAsync(string studentFirbaseId, StudentStatus newStatus,
            AccountModel account, Guid? classId = null);

        Task<bool> UpdateStudentScore(UpdateStudentScoreModel model, AccountModel account);
        Task<bool> UpdateBatchStudentClassScores(UpdateBatchStudentClassScoreModel model,
            AccountModel account);
    }
}
