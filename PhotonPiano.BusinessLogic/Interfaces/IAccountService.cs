using PhotonPiano.BusinessLogic.BusinessModel.Account;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IAccountService
{
    Task<List<AccountModel>> GetAccounts();

    Task<AccountModel> GetAndCreateAccountIfNotExistsCredentials(string firebaseId, string email,
        bool isEmailVerified = false,
        bool requireCheckingFirebaseId = true);

    Task<AccountModel> CreateAccount(string firebaseUId, string email);

    Task<AccountDetailModel> GetAccountById(string firebaseId);

    Task<bool> IsAccountExist(string firebaseId);
}