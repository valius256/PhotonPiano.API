using PhotonPiano.BusinessLogic.BusinessModel.Query;

namespace PhotonPiano.BusinessLogic.BusinessModel.Criteria;

public record QueryCriteriaModel : QueryPagedModel
{
    public string? Keyword { get; set; }

    public string GetLikeKeyword() => string.IsNullOrEmpty(Keyword) ? string.Empty : $"%{Keyword}%";
    public void Deconstruct(out int page, out int pageSize, out string sortColumn, out bool orderByDesc, out string? keyword)
    {
        page = Page;
        pageSize = PageSize;
        sortColumn = SortColumn;
        orderByDesc = OrderByDesc;
        keyword = Keyword;
    }
}