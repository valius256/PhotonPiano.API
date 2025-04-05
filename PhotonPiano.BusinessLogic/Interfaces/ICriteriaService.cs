using PhotonPiano.BusinessLogic.BusinessModel.Criteria;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ICriteriaService
{
    Task<List<MinimalCriteriaModel>> GetMinimalCriterias(QueryMinimalCriteriasModel queryModel);

    Task<PagedResult<CriteriaModel>> GetPagedCriteria(QueryCriteriaModel query);

    Task<CriteriaDetailModel> GetCriteriaDetailById(Guid id);

    Task<CriteriaModel> CreateCriteria(CreateCriteriaModel createCriteriaModel, string userFirebaseId);

    Task UpdateCriteria(BulkUpdateCriteriaModel updateCriteriaModel, string userFirebaseId);

    Task DeleteCriteria(Guid id, string userFirebaseId);

    Task<List<CriteriaGradeModel>> GetAllCriteriaDetails(Guid classId,
        CriteriaFor criteriaType = CriteriaFor.Class);
}