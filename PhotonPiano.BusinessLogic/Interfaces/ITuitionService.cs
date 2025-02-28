using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Payment;
using PhotonPiano.BusinessLogic.BusinessModel.Tuition;
using PhotonPiano.BusinessLogic.BusinessModel.Tution;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ITuitionService
{
    Task<string> PayTuition(AccountModel currentAccount, Guid tuitionId, string returnUrl, string ipAddress,
        string apiBaseUrl);

    Task HandleTuitionPaymentCallback(VnPayCallbackModel callbackModel, string accountId);

    Task<PagedResult<TuitionWithStudentClassModel>> GetTuitionsPaged(QueryTuitionModel queryTuitionModel,
        AccountModel? account = default);

    Task<TuitionWithStudentClassModel> GetTuitionById(Guid tuitionId);

    Task CreateTuitionWhenRegisterClass(List<StudentClass> models, List<Slot> slots);

    Task<decimal> GetTuitionRefundAmount(string studentId, Guid? classId);

    // Cron job
    Task CronAutoCreateTuition();
    Task CronForTuitionReminder();
}