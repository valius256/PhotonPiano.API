using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;

namespace PhotonPiano.BusinessLogic.BusinessModel.Account;

public record AccountDetailModel : AccountModel
{
    public ClassModel? CurrentClass { get; init; }
    public List<StudentClassWithClassModel> StudentClasses { get; set; } = [];
    public List<EntranceTestStudentModel> EntranceTestStudents { get; init; } = [];
}