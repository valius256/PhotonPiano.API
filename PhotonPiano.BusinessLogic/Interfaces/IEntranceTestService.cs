using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestResult;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;
using PhotonPiano.BusinessLogic.BusinessModel.Payment;
using PhotonPiano.BusinessLogic.BusinessModel.Query;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IEntranceTestService
{
    Task<PagedResult<EntranceTestDetailModel>> GetPagedEntranceTest(QueryEntranceTestModel query, AccountModel currentAccount);

    Task<EntranceTestDetailModel> GetEntranceTestDetailById(Guid id, AccountModel currentAccount);

    Task<EntranceTestDetailModel> CreateEntranceTest(CreateEntranceTestModel entranceTestStudentModel,
        AccountModel currentAccount);

    Task DeleteEntranceTest(Guid id, string? currentUserFirebaseId = default);

    Task UpdateEntranceTest(Guid id, UpdateEntranceTestModel entranceTestStudentModel,
        string? currentUserFirebaseId = default);

    Task<PagedResult<EntranceTestStudentDetail>>
        GetPagedEntranceTestStudent(QueryPagedModel query, Guid entranceTestId, AccountModel currentAccount);

    Task<EntranceTestStudentDetail> GetEntranceTestStudentDetail(Guid entranceTestId, string studentId,
        AccountModel currentAccount);

    Task<string> EnrollEntranceTest(AccountModel currentAccount, string returnUrl, string ipAddress, string apiBaseUrl);

    Task HandleEnrollmentPaymentCallback(VnPayCallbackModel callbackModel, string accountId);

    Task AutoArrangeEntranceTests(AutoArrangeEntranceTestsModel model, AccountModel currentAccount);

    Task UpdateStudentsEntranceTestResults(UpdateStudentsEntranceTestResultsModel updateModel, Guid entranceTestId, AccountModel currentAccount);

    Task UpdateStudentEntranceResults(Guid id, string studentId, UpdateEntranceTestResultsModel updateModel, AccountModel currentAccount);

    Task UpdateEntranceTestsMaxStudents(int maxStudents, AccountModel currentAccount);
}