using System.Linq.Expressions;
using Mapster;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.Services;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;

    public AccountService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AccountModel> CreateAccount(string firebaseUid, string email)
    {
        var account = await _unitOfWork.AccountRepository.FindFirstAsync(x => x.AccountFirebaseId == firebaseUid);

        if (account is not null)
        {
            return account.Adapt<AccountModel>();
        }

        var newAccount = new Account
        {
            Email = email,
            AccountFirebaseId = firebaseUid,
            Role = Role.Student,
            RecordStatus = RecordStatus.IsActive,
            IsEmailVerified = false,
        };

        await _unitOfWork.AccountRepository.AddAsync(newAccount);

        await _unitOfWork.SaveChangesAsync();

        return newAccount.Adapt<AccountModel>();
    }


    public async Task<List<AccountModel>> GetAccounts()
    {
        return await _unitOfWork.AccountRepository.FindProjectedAsync<AccountModel>(hasTrackings: false);
    }

    public async Task<AccountModel> GetAndCreateAccountIfNotExistsCredentials(string firebaseId, string email,
        bool isEmailVerified = false, bool requireCheckingFirebaseId = true)
    {
        Expression<Func<Account, bool>> expression = requireCheckingFirebaseId
            ? a => a.AccountFirebaseId == firebaseId && a.Email == email
            : a => a.Email == email;

        var account = await _unitOfWork.AccountRepository.FindFirstProjectedAsync<AccountModel>(expression);

        if (account is not null)
        {
            return account;
        }

        var newAccount = new Account
        {
            Email = email,
            AccountFirebaseId = firebaseId,
            Role = Role.Student,
            RecordStatus = RecordStatus.IsActive,
            IsEmailVerified = isEmailVerified
        };

        await _unitOfWork.AccountRepository.AddAsync(newAccount);

        await _unitOfWork.SaveChangesAsync();

        return newAccount.Adapt<AccountModel>();
    }
}