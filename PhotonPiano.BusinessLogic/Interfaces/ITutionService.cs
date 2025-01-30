using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Payment;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ITutionService
{
    Task<string> PayTuition(AccountModel currentAccount, Guid tutionId, string returnUrl, string ipAddress,
        string apiBaseUrl);

    Task HandleTutionPaymentCallback(VnPayCallbackModel callbackModel, string accountId);

    Task CronAutoCreateTution();
}