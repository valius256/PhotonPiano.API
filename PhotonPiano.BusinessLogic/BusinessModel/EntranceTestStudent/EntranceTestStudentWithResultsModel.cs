using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestResult;
using PhotonPiano.BusinessLogic.BusinessModel.Level;

namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;

public record EntranceTestStudentWithResultsModel : EntranceTestStudentModel
{
    public ICollection<EntranceTestResultModel> EntranceTestResults { get; init; } = [];

    public LevelModel? Level { get; init; }
}