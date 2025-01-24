using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;
using PhotonPiano.BusinessLogic.BusinessModel.Payment;
using PhotonPiano.BusinessLogic.BusinessModel.Query;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IEntranceTestService
{
    Task<PagedResult<EntranceTestDetailModel>> GetPagedEntranceTest(QueryEntranceTestModel query);

    Task<EntranceTestDetailModel> GetEntranceTestDetailById(Guid id);

    Task<EntranceTestDetailModel> CreateEntranceTest(CreateEntranceTestModel entranceTestStudentModel,
        string? currentUserFirebaseId = default);

    Task DeleteEntranceTest(Guid id, string? currentUserFirebaseId = default);

    Task UpdateEntranceTest(Guid id, UpdateEntranceTestModel entranceTestStudentModel,
        string? currentUserFirebaseId = default);

    Task<PagedResult<EntranceTestStudentDetail>>
        GetPagedEntranceTestStudent(QueryPagedModel query, Guid entranceTestId, AccountModel currentAccount);

    Task<EntranceTestStudentDetail> GetEntranceTestStudentDetail(Guid entranceTestId, string studentId,
        AccountModel currentAccount);

    Task<string> EnrollEntranceTest(AccountModel currentAccount, string returnUrl, string ipAddress, string apiBaseUrl);

    Task HandleEnrollmentPaymentCallback(VnPayCallbackModel callbackModel, string accountId);
}