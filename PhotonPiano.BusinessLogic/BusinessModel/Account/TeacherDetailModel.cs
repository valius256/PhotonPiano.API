using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;

namespace PhotonPiano.BusinessLogic.BusinessModel.Account;

public record TeacherDetailModel : AccountModel
{
    public List<EntranceTestModel> InstructorEntranceTests { get; init; } = [];
    public List<ClassModel> InstructorClasses { get; init; } = [];
}