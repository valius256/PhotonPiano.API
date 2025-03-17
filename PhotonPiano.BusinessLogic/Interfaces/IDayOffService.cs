using PhotonPiano.BusinessLogic.BusinessModel.DayOff;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Interfaces
{
    public interface IDayOffService
    {
        Task<PagedResult<DayOffModel>> GetPagedDayOffs(QueryDayOffModel queryDayOff);
    }
}
