using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IEntranceTestStudentService
{
    Task<PagedResult<EntranceTestStudentWithEntranceTestAndStudentAccountModel>> GetPagedEntranceTest(QueryEntranceTestStudentModel query);

    Task<EntranceTestStudentWithEntranceTestAndStudentAccountModel> GetEntranceTestStudentDetailById(Guid id);
    
    Task<EntranceTestStudentWithEntranceTestAndStudentAccountModel> CreateEntranceTestStudent(EntranceTestStudentModel entranceTestStudent, string? currentUserFirebaseId = default);
    
    Task DeleteEntranceTestStudent(Guid id, string? currentUserFirebaseId = default);
    
    Task UpdateEntranceTestStudent(Guid id, UpdateEntranceTestStudentModel entranceTestStudent, string? currentUserFirebaseId = default);
}