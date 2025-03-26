using PhotonPiano.Api.Responses.Account;
using PhotonPiano.Api.Responses.EntranceTestStudent;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestResult;

namespace PhotonPiano.Api.Responses.EntranceTest;

public record EntranceTestStudentDetailResponse : EntranceTestStudentResponse
{
    public EntranceTestDetailResponse? EntranceTest { get; init; }
    public AccountResponse? Student { get; init; }
    public List<EntranceTestResultModel> EntranceTestResults { get; init; } = [];
}