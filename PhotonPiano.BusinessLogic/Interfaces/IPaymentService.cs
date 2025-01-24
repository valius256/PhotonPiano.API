using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IPaymentService
{
    string CreateVnPayPaymentUrl(Transaction transaction, string ipAddress, string apiBaseUrl, string accountId, string clientRedirectUrl);
}