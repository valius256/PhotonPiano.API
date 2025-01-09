using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IEntranceTestStudentService
{
    Task<PagedResult<EntranceTestStudentDetail>> GetPagedEntranceTest(QueryEntranceTestStudentModel query);

    Task<EntranceTestStudentDetail> GetEntranceTestStudentDetailById(Guid id);
    
    Task<EntranceTestStudentDetail> CreateEntranceTestStudent(EntranceTestStudentModel entranceTestStudent, string? currentUserFirebaseId = default);
    
    Task DeleteEntranceTestStudent(Guid id, string? currentUserFirebaseId = default);
    
    Task UpdateEntranceTestStudent(Guid id, UpdateEntranceTestStudentModel entranceTestStudent, string? currentUserFirebaseId = default);
}