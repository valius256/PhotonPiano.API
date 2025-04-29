using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.Payment;
using PhotonPiano.BusinessLogic.BusinessModel.Tuition;
using PhotonPiano.BusinessLogic.BusinessModel.Tution;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ITuitionService
{
    Task<string> PayTuition(AccountModel currentAccount, PayTuitionModel model);

    Task HandleTuitionPaymentCallback(VnPayCallbackModel callbackModel, string accountId);

    Task<PagedResult<TuitionWithStudentClassModel>> GetTuitionsPaged(QueryTuitionModel queryTuitionModel,
        AccountModel? account = default);

    Task<TuitionWithStudentClassModel> GetTuitionById(Guid tuitionId, AccountModel? currentAccount);

    Task CreateTuitionWhenRegisterClass(ClassDetailModel classDetailModel);

    Task<decimal> GetTuitionRefundAmount(string studentId, Guid? classId);

    // Cron job
    Task CronAutoCreateTuition();
    Task CronForTuitionReminder();
    Task CronForTuitionOverdue();
}