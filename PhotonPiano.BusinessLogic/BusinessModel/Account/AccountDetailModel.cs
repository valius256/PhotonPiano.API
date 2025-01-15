using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;

namespace PhotonPiano.BusinessLogic.BusinessModel.Account;

public record AccountDetailModel : AccountModel
{
    public List<EntranceTestStudentModel> EntranceTestStudents { get; init; } = [];

    public List<EntranceTestModel> EntranceTests { get; init; } = [];
}