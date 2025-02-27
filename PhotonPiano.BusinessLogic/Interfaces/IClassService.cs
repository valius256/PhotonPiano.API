using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.DataAccess.Models.Paging;
using Role = PhotonPiano.DataAccess.Models.Enum.Role;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IClassService
{
    Task<PagedResult<ClassModel>> GetPagedClasses(QueryClassModel queryClass, AccountModel account);

    Task<ClassDetailModel> GetClassDetailById(Guid id);

    Task<ClassModel> GetClassByUserFirebaseId(string userFirebaseId);

    Task<List<ClassModel>> AutoArrangeClasses(ArrangeClassModel arrangeClassModel, string userId);
    Task<byte[]> GenerateGradeTemplate(Guid classId);
    
    
}