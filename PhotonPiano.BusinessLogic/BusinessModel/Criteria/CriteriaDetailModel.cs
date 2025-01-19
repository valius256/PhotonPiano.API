using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestResult;

namespace PhotonPiano.BusinessLogic.BusinessModel.Criteria;

public record CriteriaDetailModel : CriteriaModel
{
    public AccountSimpleModel CreatedBy { get; init; } = default!;
    public List<EntranceTestResultModel> EntranceTestResults { get; set; } = [];
}