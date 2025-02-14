using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.Payment;
using PhotonPiano.BusinessLogic.BusinessModel.Tution;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ITutionService
{
    Task<string> PayTuition(AccountModel currentAccount, Guid tutionId, string returnUrl, string ipAddress,
        string apiBaseUrl);

    Task HandleTutionPaymentCallback(VnPayCallbackModel callbackModel, string accountId);

    Task CronAutoCreateTution();

    Task<PagedResult<TutionWithStudentClassModel>> GetTutionsPaged(QueryTutionModel queryTutionModel,
        AccountModel? account = default);

    Task<TutionWithStudentClassModel> GetTutionById(Guid tutionId);

    Task CreateTutionWhenRegisterClass(List<StudentClass> models);
}