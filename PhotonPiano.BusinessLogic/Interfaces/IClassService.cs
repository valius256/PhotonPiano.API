using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IClassService
{
    Task<PagedResult<ClassModel>> GetPagedClasses(QueryClassModel queryClass);

    Task<ClassDetailModel> GetClassDetailById(Guid id);

    Task<ClassModel> GetClassByUserFirebaseId(string userFirebaseId);

    Task<List<ClassModel>> AutoArrangeClasses(ArrangeClassModel arrangeClassModel);
}