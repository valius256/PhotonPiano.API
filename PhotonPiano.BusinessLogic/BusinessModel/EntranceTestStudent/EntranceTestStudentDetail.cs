using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestResult;

namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;

public record EntranceTestStudentDetail : EntranceTestStudentModel
{
    public EntranceTestModel? EntranceTest { get; init; }
    public AccountSimpleModel? Student { get; init; }
    public List<EntranceTestResultModel> EntranceTestResults { get; set; } = [];
}