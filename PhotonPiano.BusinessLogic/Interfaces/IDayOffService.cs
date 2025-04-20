using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.DayOff;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Interfaces
{
    public interface IDayOffService
    {
        Task<PagedResult<DayOffModel>> GetPagedDayOffs(QueryDayOffModel queryDayOff);

        Task<DayOffModel> CreateDayOff(CreateDayOffModel createModel, AccountModel currentAccount);

        Task UpdateDayOff(Guid id, UpdateDayOffModel updateModel, AccountModel currentAccount);
        
        Task DeleteDayOff(Guid id, AccountModel currentAccount);
    }
}
