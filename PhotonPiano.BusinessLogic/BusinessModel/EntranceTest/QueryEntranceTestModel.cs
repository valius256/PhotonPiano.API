using PhotonPiano.BusinessLogic.BusinessModel.Query;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;

public record QueryEntranceTestModel : QueryPagedModel
{
    public List<Guid>? RoomIds { get; init; } = [];
    public string? Keyword { get; init; }
    public List<Shift>? Shifts { get; init; } = [];
    public List<Guid>? EntranceTestIds { get; init; } = [];
    public bool? IsAnnouncedScore { get; init; }

    public List<string>? InstructorIds { get; init; } = [];

    public string GetLikeKeyword()
    {
        return string.IsNullOrEmpty(Keyword) ? string.Empty : $"%{Keyword}%";
    }

    public void Deconstruct(out int page, out int pageSize, out string sortColumn, out bool orderByDesc,
        out List<Guid>? roomIds, out string? keyword, out List<Shift>? shifts, out List<Guid>? entranceTestIds,
        out bool? isAnnouncedScore, out List<string>? instructorIds)
    {
        page = Page;
        pageSize = PageSize;
        sortColumn = SortColumn;
        orderByDesc = OrderByDesc;
        roomIds = RoomIds;
        keyword = Keyword;
        shifts = Shifts;
        entranceTestIds = EntranceTestIds;
        isAnnouncedScore = IsAnnouncedScore;
        instructorIds = InstructorIds;
    }

    public void Deconstruct(out List<Guid>? roomIds, out string? keyword, out List<Shift>? shifts,
        out List<Guid>? entranceTestIds, out bool? isAnnouncedScore, out List<string>? instructorIds)
    {
        roomIds = RoomIds;
        keyword = Keyword;
        shifts = Shifts;
        entranceTestIds = EntranceTestIds;
        isAnnouncedScore = IsAnnouncedScore;
        instructorIds = InstructorIds;
    }
}