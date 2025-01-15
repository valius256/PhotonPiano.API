using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IEntranceTestService
{
    Task<PagedResult<EntranceTestDetailModel>> GetPagedEntranceTest(QueryEntranceTestModel query);

    Task<EntranceTestDetailModel> GetEntranceTestDetailById(Guid id);

    Task<EntranceTestDetailModel> CreateEntranceTest(CreateEntranceTestModel entranceTestStudentModel,
        string? currentUserFirebaseId = default);

    Task DeleteEntranceTest(Guid id, string? currentUserFirebaseId = default);

    Task UpdateEntranceTest(Guid id, UpdateEntranceTestModel entranceTestStudentModel,
        string? currentUserFirebaseId = default);
}