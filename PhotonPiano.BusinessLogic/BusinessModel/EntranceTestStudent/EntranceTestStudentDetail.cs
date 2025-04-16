using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestResult;
using PhotonPiano.BusinessLogic.BusinessModel.Level;

namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;

public record EntranceTestStudentDetail : EntranceTestStudentModel
{
    public EntranceTestWithInstructorModel? EntranceTest { get; init; }
    public AccountSimpleModel? Student { get; init; }
    public List<EntranceTestResultWithCriteriaModel> EntranceTestResults { get; set; } = [];
    public LevelModel? Level { get; set; }
}