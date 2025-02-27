using PhotonPiano.BusinessLogic.BusinessModel.Criteria;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ICriteriaService
{
    Task<List<MinimalCriteriaModel>> GetMinimalCriterias(QueryMinimalCriteriasModel queryModel);
    
    Task<PagedResult<CriteriaDetailModel>> GetPagedCriteria(QueryCriteriaModel query);

    Task<CriteriaDetailModel> GetCriteriaDetailById(Guid id);
}