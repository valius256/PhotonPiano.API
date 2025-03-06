using PhotonPiano.BusinessLogic.BusinessModel.Class;

namespace PhotonPiano.BusinessLogic.Interfaces
{
    public interface IStudentClassService
    {
        Task<List<StudentClassModel>> CreateStudentClass(CreateStudentClassModel createStudentClassModel, string accountFirebaseId);

        Task ChangeClassOfStudent(ChangeClassModel changeClassModel, string accountFirebaseId);

        Task DeleteStudentClass(string studentId, Guid classId, bool isExpelled, string accountFirebaseId);
    }
}
