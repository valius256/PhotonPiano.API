using Mapster;
using Newtonsoft.Json;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Application;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services;

public class ApplicationService : IApplicationService
{
    private readonly IServiceFactory _serviceFactory;
    private readonly IUnitOfWork _unitOfWork;

    public ApplicationService(IUnitOfWork unitOfWork, IServiceFactory serviceFactory)
    {
        _unitOfWork = unitOfWork;
        _serviceFactory = serviceFactory;
    }

    public async Task<PagedResult<ApplicationModel>> GetPagedApplications(QueryPagedApplicationModel model,
        AccountModel currentAccount)
    {
        var (page, pageSize, sortColumn, orderByDesc, keyword,
            types, statuses) = model;

        return await _unitOfWork.ApplicationRepository.GetPaginatedWithProjectionAsync<ApplicationModel>(
            page, pageSize, sortColumn, orderByDesc,
            expressions:
            [
                a => currentAccount.Role == Role.Staff || a.CreatedById == currentAccount.AccountFirebaseId,
                a => types.Count == 0 || types.Contains(a.Type),
                a => statuses.Count == 0 || statuses.Contains(a.Status)
            ]);
    }


    public async Task<ApplicationDetailsModel> SendAnApplication(SendApplicationModel sendModel,
        AccountModel currentAccount)
    {
        var application = sendModel.Adapt<Application>();

        application.CreatedById = currentAccount.AccountFirebaseId;
        application.CreatedByEmail = currentAccount.Email;
        application.CreatedAt = DateTime.UtcNow.AddHours(7); 
        application.Status = ApplicationStatus.Pending;

        if (sendModel.File is not null)
        {
            application.FileUrl = await _serviceFactory.PinataService.UploadFile(sendModel.File);
        }

        await _unitOfWork.ApplicationRepository.AddAsync(application);
        await _unitOfWork.SaveChangesAsync();

        await NotifyStaffsAsync(application, currentAccount);

        return await GetApplicationDetailsAsync(application.Id);
    }


    public async Task UpdateApplicationStatus(Guid id, UpdateApplicationModel updateModel, AccountModel currentAccount)
    {
        var application = await _unitOfWork.ApplicationRepository.FindSingleAsync(a => a.Id == id);

        if (application is null)
        {
            throw new NotFoundException("Application not found");
        }

        if (application.Status == ApplicationStatus.Approved)
        {
            throw new BadRequestException("Application is already approved");
        }

        updateModel.Adapt(application);
        application.UpdatedAt = DateTime.UtcNow.AddHours(7);
        application.UpdatedById = currentAccount.AccountFirebaseId;
        application.UpdatedByEmail = currentAccount.Email;

        if (updateModel.Status == ApplicationStatus.Approved)
        {
            application.ApprovedById = currentAccount.AccountFirebaseId;
            application.ApprovedByEmail = currentAccount.Email;
            application.ApprovedAt = DateTime.UtcNow.AddHours(7);
        }

        await _unitOfWork.SaveChangesAsync();

        //Push noti here
        await _serviceFactory.NotificationService.SendNotificationAsync(application.CreatedById,
            $"Your application just got approved!", "");
    }

    public async Task<ApplicationDetailsModel> SendRefundApplication(SendRefundApplicationModel sendModel,
        AccountModel currentAccount)
    {
        var getAmount = await _serviceFactory.TuitionService.GetTuitionRefundAmount(currentAccount.AccountFirebaseId, currentAccount.CurrentClassId);

        // convert to json 
        var bankInformation = new
        {
            sendModel.BankName,
            sendModel.BankAccountName,
            sendModel.BankAccountNumber,
            getAmount
        };

        var application = sendModel.Adapt<Application>();

        application.CreatedById = currentAccount.AccountFirebaseId;
        application.CreatedByEmail = currentAccount.Email;
        application.CreatedAt = DateTime.UtcNow.AddHours(7);
        application.Status = ApplicationStatus.Pending;
        application.AdditionalData = JsonConvert.SerializeObject(bankInformation);

        if (sendModel.File is not null)
        {
            var fileUrl = await _serviceFactory.PinataService.UploadFile(sendModel.File);
            application.FileUrl = fileUrl;
        }

        Application? createdApplication = null;
        
        // if student have class, remove student from class
        if (currentAccount.CurrentClassId is not null)
        {
            var result = _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _unitOfWork.SlotStudentRepository.ExecuteDeleteAsync(x =>
                    x.StudentFirebaseId == currentAccount.AccountFirebaseId);

                await _unitOfWork.StudentClassRepository.ExecuteDeleteAsync(x =>
                    x.StudentFirebaseId == currentAccount.AccountFirebaseId);

                await _unitOfWork.AccountRepository.ExecuteUpdateAsync(
                    account => account.AccountFirebaseId == currentAccount.AccountFirebaseId,
                    set => set
                        .SetProperty(account => account.CurrentClassId, (Guid?)null)
                        .SetProperty(account => account.StudentStatus, StudentStatus.Leave)
                );

                createdApplication = await _unitOfWork.ApplicationRepository.AddAsync(application);
            });

            await Task.WhenAll(result);
        }

        await NotifyStaffsAsync(application, currentAccount);

        if (currentAccount.CurrentClassId != null)
            await _serviceFactory.RedisCacheService.DeleteByPatternAsync($"*{currentAccount.CurrentClassId}*");

        return createdApplication.Adapt<ApplicationDetailsModel>();
    }

    private async Task NotifyStaffsAsync(Application application, AccountModel currentAccount)
    {
        var staffs = await _unitOfWork.AccountRepository.FindAsync(a => a.Role == Role.Staff, false);

        List<Task> pushNotiTasks = staffs.Select(staff =>
            _serviceFactory.NotificationService.SendNotificationAsync(
                staff.AccountFirebaseId,
                $"Learner {currentAccount.FullName} has just sent {application.Type}",
                $"Details: {application.Reason}"
            )).ToList();

        await Task.WhenAll(pushNotiTasks);
    }

    protected async Task<ApplicationDetailsModel> GetApplicationDetailsAsync(Guid applicationId)
    {
        return (await _unitOfWork.ApplicationRepository.FindSingleProjectedAsync<ApplicationDetailsModel>(
            a => a.Id == applicationId,
            false,
            option: TrackingOption.IdentityResolution))!;
    }

}