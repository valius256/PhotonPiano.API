using Mapster;
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
    private readonly IUnitOfWork _unitOfWork;

    public ApplicationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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
                a => statuses.Count == 0 || statuses.Contains(a.Status),
            ]);
    }

    public async Task<ApplicationDetailsModel> SendAnApplication(SendApplicationModel sendModel,
        AccountModel currentAccount)
    {
        var application = sendModel.Adapt<Application>();

        // application.Id = Guid.CreateVersion7();
        application.CreatedById = currentAccount.AccountFirebaseId;
        application.CreatedByEmail = currentAccount.Email;
        application.CreatedAt = DateTime.UtcNow;
        application.Status = ApplicationStatus.Pending;

        await _unitOfWork.ApplicationRepository.AddAsync(application);
        await _unitOfWork.SaveChangesAsync();

        //Push noti here

        return (await _unitOfWork.ApplicationRepository.FindSingleProjectedAsync<ApplicationDetailsModel>(
            a => a.Id == application.Id,
            hasTrackings: false,
            option: TrackingOption.IdentityResolution))!;
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
        application.UpdatedAt = DateTime.UtcNow;
        application.UpdatedById = currentAccount.AccountFirebaseId;
        application.UpdatedByEmail = currentAccount.Email;

        if (updateModel.Status == ApplicationStatus.Approved)
        {
            application.ApprovedById = currentAccount.AccountFirebaseId;
            application.ApprovedByEmail = currentAccount.Email;
            application.ApprovedAt = DateTime.UtcNow;
        }
        
        await _unitOfWork.SaveChangesAsync();
        
        //Push noti here

    }
}