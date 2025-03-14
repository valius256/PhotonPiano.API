using System.Linq.Expressions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IServiceFactory _serviceFactory;

    public AccountService(IUnitOfWork unitOfWork, IServiceFactory serviceFactory)
    {
        _unitOfWork = unitOfWork;
        _serviceFactory = serviceFactory;
    }

    public async Task<AccountModel> CreateAccount(string firebaseUId, string email)
    {
        var account = await _unitOfWork.AccountRepository.FindFirstAsync(x => x.AccountFirebaseId == firebaseUId);

        if (account is not null) return account.Adapt<AccountModel>();

        var newAccount = new Account
        {
            Email = email,
            AccountFirebaseId = firebaseUId,
            Role = Role.Student,
            RecordStatus = RecordStatus.IsActive,
            IsEmailVerified = false
        };

        await _unitOfWork.AccountRepository.AddAsync(newAccount);

        await _unitOfWork.SaveChangesAsync();

        return newAccount.Adapt<AccountModel>();
    }

    public async Task<AccountDetailModel> GetAccountById(string firebaseId)
    {
        var result = await _unitOfWork.AccountRepository.FindFirstAsync(x => x.AccountFirebaseId == firebaseId);
        if (result is null) throw new NotFoundException($"Account with ID: {firebaseId} not found.");
        return result.Adapt<AccountDetailModel>();
    }

    public async Task<bool> IsAccountExist(string firebaseId)
    {
        var result = await _unitOfWork.AccountRepository
            .FindFirstAsync(e => e.AccountFirebaseId == firebaseId, false);
        if (result is null) throw new NotFoundException("Account not found.");
        return true;
    }

    public async Task UpdateAccount(UpdateAccountModel model, AccountModel currentAccount, string idToken)
    {
        var account =
            await _unitOfWork.AccountRepository.FindSingleAsync(a =>
                a.AccountFirebaseId == currentAccount.AccountFirebaseId);

        if (account is null)
        {
            throw new NotFoundException("Account not found.");
        }

        if (!string.IsNullOrEmpty(model.UserName) &&
            model.UserName != currentAccount.UserName &&
            await _unitOfWork.AccountRepository.AnyAsync(a => a.UserName == model.UserName)
           )
        {
            throw new ConflictException("Username already exists.");
        }

        if (!string.IsNullOrEmpty(model.Email) && model.Email != currentAccount.Email)
        {
            if (await _unitOfWork.AccountRepository.AnyAsync(a => a.Email == model.Email))
            {
                throw new ConflictException("Email already exists.");
            }

            await _serviceFactory.AuthService.UpdateFirebaseEmail(idToken, model.Email);
        }

        model.Adapt(account);

        await _unitOfWork.SaveChangesAsync();
    }


    public async Task<PagedResult<AccountModel>> GetAccounts(AccountModel currentAccount, QueryPagedAccountsModel model)
    {
        var (page, size, column, desc, q, roles, levels, studentStatuses) = model;

        var pagedResult = await _unitOfWork.AccountRepository.GetPaginatedWithProjectionAsync<AccountModel>(
            page,
            size,
            column == "Id" ? "AccountFirebaseId" : column,
            desc,
            expressions:
            [
                GetAccountsFilterExpression(currentAccount.Role),
                a => roles.Count == 0 || roles.Contains(a.Role),
                a => string.IsNullOrEmpty(q) || a.Email.ToLower().Contains(q.ToLower()),
                a => levels.Count == 0 || !a.LevelId.HasValue || levels.Contains(a.LevelId.Value),
                a => studentStatuses.Count == 0 ||
                     a.StudentStatus != null && studentStatuses.Contains(a.StudentStatus.Value)
            ]
        );

        return pagedResult;
    }

    public async Task<List<AwaitingLevelsModel>> GetWaitingStudentOfAllLevels()
    {
        return await _unitOfWork.AccountRepository.Entities
            .Where(a => a.StudentStatus == StudentStatus.WaitingForClass)
            .GroupBy(a => a.LevelId)
            .Select(g => new AwaitingLevelsModel { Level = g.Key, Count = g.Count() })
            .ToListAsync();
    }

    public async Task<AccountModel> GetAndCreateAccountIfNotExistsCredentials(string firebaseId, string email,
        bool isEmailVerified = false, bool requireCheckingFirebaseId = true)
    {
        Expression<Func<Account, bool>> expression = requireCheckingFirebaseId
            ? a => a.AccountFirebaseId == firebaseId && a.Email == email
            : a => a.Email == email;

        var account = await _unitOfWork.AccountRepository.FindFirstProjectedAsync<AccountModel>(expression);

        if (account is not null) return account;

        var newAccount = new Account
        {
            Email = email,
            AccountFirebaseId = firebaseId,
            Role = Role.Student,
            UserName = email.Split('@')[0],
            RecordStatus = RecordStatus.IsActive,
            IsEmailVerified = isEmailVerified
        };

        await _unitOfWork.AccountRepository.AddAsync(newAccount);

        await _unitOfWork.SaveChangesAsync();

        return newAccount.Adapt<AccountModel>();
    }

    private static Expression<Func<Account, bool>> GetAccountsFilterExpression(Role role)
    {
        return role switch
        {
            Role.Instructor => a => a.Role == Role.Student,
            Role.Staff => a => a.Role != Role.Administrator,
            Role.Administrator => a => true,
            _ => a => a.Role == role
        };
    }
}