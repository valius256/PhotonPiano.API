using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Auth;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IAccountService
{
    Task<PagedResult<AccountModel>> GetAccounts(AccountModel currentAccount, QueryPagedAccountsModel model);

    Task<PagedResult<AccountModel>> GetTeachers(QueryPagedAccountsModel model);

    Task<List<AwaitingLevelsModel>> GetWaitingStudentOfAllLevels();
    
    Task<AccountModel> GetAccountFromIdAndEmail(string accountId, string email);

    Task<AccountModel> GetAndCreateAccountIfNotExistsCredentials(string firebaseId, string email,
        bool isEmailVerified = false,
        bool requireCheckingFirebaseId = true);

    Task<AccountModel> CreateAccount(string firebaseUId, string email);

    Task<AccountDetailModel> GetAccountById(string accountId);

    Task<TeacherDetailModel> GetTeacherDetailById(string firebaseId);

    Task<bool> IsAccountExist(string firebaseId);

    Task UpdateAccount(UpdateAccountModel model, AccountModel currentAccount);

    Task ChangeRole(GrantRoleModel grantRoleModel);

    Task<AccountModel> CreateNewStaff(CreateSystemAccountModel createSystemAccountModel);
    Task<AccountModel> CreateNewTeacher(CreateSystemAccountModel createSystemAccountModel);
    Task<AccountModel> UpdateContinuingLearningStatus(string firebaseId, bool wantToContinue);
}