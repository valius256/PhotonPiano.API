using PhotonPiano.BusinessLogic.BusinessModel.Payment;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ITutionService
{
    Task<string> PayTuition(string userFirebaseId, Guid tutionId, string returnUrl, string ipAddress,
        string apiBaseUrl);

    Task HandleTutionPaymentCallback(VnPayCallbackModel callbackModel, string accountId);

    Task CronAutoCreateTution();
}