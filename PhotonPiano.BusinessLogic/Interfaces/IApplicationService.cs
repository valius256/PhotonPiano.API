using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Application;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IApplicationService
{
    Task<PagedResult<ApplicationModel>> GetPagedApplications(QueryPagedApplicationModel model, AccountModel currentAccount);
    Task<ApplicationDetailsModel> SendAnApplication(SendApplicationModel sendModel, AccountModel currentAccount);
    Task UpdateApplicationStatus(Guid id, UpdateApplicationModel updateModel, AccountModel currentAccount);


}