using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IAccountService
{
    Task<PagedResult<AccountModel>> GetAccounts(AccountModel currentAccount, QueryPagedAccountsModel model);

    Task<AccountModel> GetAndCreateAccountIfNotExistsCredentials(string firebaseId, string email,
        bool isEmailVerified = false,
        bool requireCheckingFirebaseId = true);

    Task<AccountModel> CreateAccount(string firebaseUId, string email);

    Task<AccountDetailModel> GetAccountById(string firebaseId);

    Task<bool> IsAccountExist(string firebaseId);

    Task UpdateAccount(UpdateAccountModel model, AccountModel currentAccount, string idToken);
}