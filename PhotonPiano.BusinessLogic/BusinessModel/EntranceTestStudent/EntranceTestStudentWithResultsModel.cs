using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestResult;

namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;

public record EntranceTestStudentWithResultsModel : EntranceTestStudentModel
{
    public ICollection<EntranceTestResultModel> EntranceTestResults { get; init; } = [];
}