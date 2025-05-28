using FluentEmail.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Auth;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.Level;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;
using PhotonPiano.Shared.Exceptions;
using PhotonPiano.Shared.Utils;
using System.Linq.Expressions;

namespace PhotonPiano.BusinessLogic.Services;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly IServiceFactory _serviceFactory;

    public AccountService(IUnitOfWork unitOfWork, IServiceFactory serviceFactory, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _serviceFactory = serviceFactory;
        _configuration = configuration;
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

    public async Task<AccountDetailModel> GetAccountById(string accountId)
    {
        var result =
            await _unitOfWork.AccountRepository.FindSingleProjectedAsync<AccountDetailModel>(x =>
                    x.AccountFirebaseId == accountId,
                hasTrackings: false,
                option: TrackingOption.IdentityResolution,
                hasSplitQuery: true);

        if (result is null)
        {
            throw new NotFoundException($"Account with ID: {accountId} not found.");
        }

        return result;
    }

    public async Task<TeacherDetailModel> GetTeacherDetailById(string firebaseId)
    {
        var result =
            await _unitOfWork.AccountRepository.FindSingleProjectedAsync<TeacherDetailModel>(x =>
                x.AccountFirebaseId == firebaseId);
        if (result is null) throw new NotFoundException($"Account with ID: {firebaseId} not found.");
        return result;
    }

    public async Task<bool> IsAccountExist(string firebaseId)
    {
        var result = await _unitOfWork.AccountRepository
            .FindFirstAsync(e => e.AccountFirebaseId == firebaseId, false);
        if (result is null) throw new NotFoundException("Account not found.");
        return true;
    }

    public async Task UpdateAccount(UpdateAccountModel model, AccountModel currentAccount)
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
        }

        model.Adapt(account);

        await _unitOfWork.SaveChangesAsync();
    }


    public async Task<PagedResult<AccountModel>> GetAccounts(AccountModel currentAccount, QueryPagedAccountsModel model)
    {
        var (page, size, column, desc, q, roles, levels, studentStatuses, accountStatuses) = model;

        var likeKeyword = model.GetLikeKeyword();

        var pagedResult = await _unitOfWork.AccountRepository.GetPaginatedWithProjectionAsync<AccountModel>(
            page,
            size,
            column == "Id" ? "AccountFirebaseId" : column,
            desc,
            expressions:
            [
                GetAccountsFilterExpression(currentAccount.Role),
                a => string.IsNullOrEmpty(q) ||
                     EF.Functions.ILike(EF.Functions.Unaccent(a.Email), likeKeyword) ||
                     EF.Functions.ILike(EF.Functions.Unaccent(a.FullName ?? string.Empty), likeKeyword) ||
                     EF.Functions.ILike(EF.Functions.Unaccent(a.UserName ?? string.Empty), likeKeyword),
                a => roles.Count == 0 || roles.Contains(a.Role),
                a => levels.Count == 0 || (a.LevelId.HasValue && levels.Contains(a.LevelId.Value)),
                a => studentStatuses.Count == 0 ||
                     (a.StudentStatus != null && studentStatuses.Contains(a.StudentStatus.Value)),
                a => accountStatuses.Count == 0 || accountStatuses.Contains(a.Status)
            ]
        );

        return pagedResult;
    }

    public async Task<PagedResult<AccountModel>> GetTeachers(QueryPagedAccountsModel model)
    {
        var (page, size, column, desc, q, roles, levels, studentStatuses, accountStatuses) = model;

        var pagedResult = await _unitOfWork.AccountRepository.GetPaginatedWithProjectionAsync<AccountModel>(
            page,
            size,
            column == "Id" ? "AccountFirebaseId" : column,
            desc,
            expressions:
            [
                a => a.Role == Role.Instructor,
                a => string.IsNullOrEmpty(q) || a.Email.ToLower().Contains(q.ToLower()),
                a => levels.Count == 0 || !a.LevelId.HasValue || levels.Contains(a.LevelId.Value),
                a => accountStatuses.Count == 0 || accountStatuses.Contains(a.Status)
            ]
        );

        return pagedResult;
    }

    public async Task<List<AwaitingLevelsModel>> GetWaitingStudentOfAllLevels()
    {
        return await _unitOfWork.AccountRepository.Entities
            .Where(a => a.StudentStatus == StudentStatus.WaitingForClass)
            .GroupBy(a => a.Level)
            .Select(g => new AwaitingLevelsModel { Level = g.Key.Adapt<LevelModel>(), Count = g.Count() })
            .ToListAsync();
    }

    public async Task<AccountModel> GetAccountFromIdAndEmail(string accountId, string email)
    {
        var account = await _unitOfWork.AccountRepository.FindSingleProjectedAsync<AccountModel>(a =>
            a.AccountFirebaseId == accountId
            && a.Email == email);
        if (account is null)
        {
            throw new UnauthorizedException("Account not found.");
        }

        return account;
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

    public async Task ChangeRole(GrantRoleModel grantRoleModel)
    {
        var account =
            await _unitOfWork.AccountRepository.FindFirstAsync(a =>
                a.AccountFirebaseId == grantRoleModel.AccountFirebaseId);
        if (account == null)
        {
            throw new NotFoundException("Account not found");
        }

        if (account.Role != Role.Staff && account.Role != Role.Administrator)
        {
            throw new BadRequestException("Not applicable");
        }

        account.Role = grantRoleModel.Role;
        await _unitOfWork.AccountRepository.UpdateAsync(account);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<AccountModel> CreateNewStaff(CreateSystemAccountModel createSystemAccountModel)
    {
        return await CreateSystemAccount(createSystemAccountModel, Role.Staff);
    }

    public async Task<AccountModel> CreateNewTeacher(CreateSystemAccountModel createSystemAccountModel)
    {
        return await CreateSystemAccount(createSystemAccountModel, Role.Instructor);
    }

    private async Task<AccountModel> CreateSystemAccount(CreateSystemAccountModel signUpModel, Role role)
    {
        var password = AuthUtils.GeneratePassword(16);
        //var firebaseId = await _serviceFactory.AuthService.SignUpOnFirebase(signUpModel.Email, password);

        var account = signUpModel.Adapt<Account>();
        account.Level = null; //detach
        account.Role = role;
        account.StudentStatus = null;
        account.AccountFirebaseId = Guid.NewGuid().ToString();
        account.Password = AuthUtils.HashPassword(password);
        var createAccount = await _unitOfWork.AccountRepository.AddAsync(account);
        await _unitOfWork.SaveChangesAsync();

        //Send password mail
        var resetUrl = _configuration["PasswordResetBaseUrl"];
        var emailParam = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "fullName", $"{account.ResetPasswordToken}" },
            { "email", $"{account.Email}" },
            { "tempPassword", password },
            { "url", $"{resetUrl}sign-in" },
        };
        await _serviceFactory.EmailService.SendAsync("AccountCreated", [account.Email], [], emailParam);
        return createAccount.Adapt<AccountModel>();
    }


    public async Task<AccountModel> UpdateContinuingLearningStatus(string firebaseId, bool wantToContinue)
    {
        var account = await _unitOfWork.AccountRepository.FindSingleAsync(a =>
            a.AccountFirebaseId == firebaseId && a.Role == Role.Student);

        if (account is null)
        {
            throw new NotFoundException($"Student account with ID: {firebaseId} not found.");
        }

        account.WantToContinue = wantToContinue;
        StudentStatus? targetStatus = null;

        if (wantToContinue)
        {
            if (account.StudentStatus == StudentStatus.Leave)
            {
                targetStatus = StudentStatus.WaitingForClass;
            }
        }
        else
        {
            if (account.StudentStatus == StudentStatus.WaitingForClass ||
                account.StudentStatus == StudentStatus.InClass)
            {
                targetStatus = StudentStatus.Leave;
            }
        }

        if (targetStatus.HasValue && account.StudentStatus != targetStatus.Value)
        {
            if (!_serviceFactory.StudentClassService.IsValidStatusTransition(account.StudentStatus!.Value,
                    targetStatus.Value))
            {
                throw new BadRequestException(
                    $"Invalid status transition from {account.StudentStatus} to {targetStatus}");
            }

            account.StudentStatus = targetStatus.Value;
        }

        await _unitOfWork.SaveChangesAsync();
        return account.Adapt<AccountModel>();
    }
}