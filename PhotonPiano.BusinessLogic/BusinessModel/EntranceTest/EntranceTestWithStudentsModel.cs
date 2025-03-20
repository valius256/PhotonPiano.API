using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;

namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;

public record EntranceTestWithStudentsModel : EntranceTestModel
{
    public ICollection<EntranceTestStudentModel> EntranceTestStudents { get; init; } = [];
}