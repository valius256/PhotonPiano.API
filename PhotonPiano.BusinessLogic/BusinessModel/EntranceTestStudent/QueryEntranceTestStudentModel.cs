using PhotonPiano.BusinessLogic.BusinessModel.Query;

namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;

public record QueryEntranceTestStudentModel : QueryPagedModel
{
    public List<string>? StudentsFirebaseIds { get; init; } = [];
    public List<decimal>? BandScores { get; init; } = [];
    public List<Guid>? EntranceTestIds { get; init; } = [];


    public void Deconstruct(out int page, out int pageSize, out string sortColumn, out bool orderByDesc, out List<string>? studentsFirebaseIds, out List<decimal>? bandScores, out List<Guid>? entranceTestIds)
    {
        page = Page;
        pageSize = PageSize;
        sortColumn = SortColumn;
        orderByDesc = OrderByDesc;
        studentsFirebaseIds = StudentsFirebaseIds;
        bandScores = BandScores;
        entranceTestIds = EntranceTestIds;
    }

    public void Deconstruct(out List<string>? studentFirebaseIds, out List<decimal>? bandScores, out List<Guid>? entranceTestIds)
    {
        studentFirebaseIds = StudentsFirebaseIds;
        bandScores = BandScores;
        entranceTestIds = EntranceTestIds;
    }
}