using PhotonPiano.BusinessLogic.BusinessModel.Criteria;

namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTestResult;

public record EntranceTestResultWithCriteriaModel : EntranceTestResultModel
{
    public CriteriaModel Criteria { get; init; } = default!;
}