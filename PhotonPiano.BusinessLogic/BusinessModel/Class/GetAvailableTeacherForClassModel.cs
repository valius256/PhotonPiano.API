using PhotonPiano.BusinessLogic.BusinessModel.Query;

namespace PhotonPiano.BusinessLogic.BusinessModel.Class;

public record GetAvailableTeacherForClassModel : QueryPagedModel
{
    public Guid ClassId { get; init; }

    public string? Keyword { get; init; }

    public string GetLikeKeyword()
    {
        return string.IsNullOrEmpty(Keyword) ? string.Empty : $"%{Keyword}%";
    }
    public void Deconstruct(out int page, out int pageSize, out string sortColumn, out bool orderByDesc,
        out Guid classId, out string? keyword)
    {
        page = Page;
        pageSize = PageSize;
        sortColumn = SortColumn;
        orderByDesc = OrderByDesc;
        classId = ClassId;
        keyword = Keyword;
    }
}