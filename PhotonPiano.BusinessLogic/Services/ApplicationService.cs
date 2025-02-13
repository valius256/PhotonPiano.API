using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Application;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;

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
}