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
    Task<PagedResult<EntranceTestDetailModel>> GetPagedEntranceTest(QueryEntranceTestModel query,
        AccountModel currentAccount);

    Task<EntranceTestDetailModel> GetEntranceTestDetailById(Guid id, AccountModel currentAccount);

    Task<PagedResult<AccountModel>> GetPagedAvailableTeachersForTest(QueryPagedModelWithKeyword queryModel,
        Guid testId,
        AccountModel currentAccount);

    Task<EntranceTestDetailModel> CreateEntranceTest(CreateEntranceTestModel createModel,
        AccountModel currentAccount);

    Task DeleteEntranceTest(Guid id, string? currentUserFirebaseId = default);

    Task UpdateEntranceTest(Guid id, UpdateEntranceTestModel updateModel,
        string? currentUserFirebaseId = default);

    Task UpdateEntranceTestScoreAnnouncementStatus(Guid id, bool isAnnounced, AccountModel currentAccount);

    Task<PagedResult<EntranceTestStudentDetail>>
        GetPagedEntranceTestStudent(QueryPagedModel query, Guid entranceTestId, AccountModel currentAccount);

    Task AddStudentsToEntranceTest(Guid testId, AddStudentsToEntranceTestModel model, AccountModel currentAccount);

    Task RemoveStudentsFromTest(Guid testId, AccountModel currentAccount, params List<string> studentIds);

    Task<EntranceTestStudentDetail> GetEntranceTestStudentDetail(Guid entranceTestId, string studentId,
        AccountModel currentAccount);

    Task RemoveStudentFromTest(Guid testId, string studentId, AccountModel currentAccount);

    Task<string> EnrollEntranceTest(AccountModel currentAccount, string returnUrl, string ipAddress, string apiBaseUrl);

    Task HandleEnrollmentPaymentCallback(VnPayCallbackModel callbackModel, string accountId);

    Task AutoArrangeEntranceTests(AutoArrangeEntranceTestsModel model, AccountModel currentAccount);

    Task UpdateStudentsEntranceTestResults(UpdateStudentsEntranceTestResultsModel updateModel, Guid entranceTestId,
        AccountModel currentAccount);

    Task UpdateStudentEntranceResults(Guid id, string studentId, UpdateEntranceTestResultsModel updateModel,
        AccountModel currentAccount);

    Task UpdateEntranceTestsMaxStudents(int maxStudents, AccountModel currentAccount);

    Task<(int theoryPercentage, int practicalPercentage)> GetScorePercentagesAsync();
}