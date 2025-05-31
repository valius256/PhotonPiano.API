using PhotonPiano.BusinessLogic.BusinessModel.Query;

namespace PhotonPiano.BusinessLogic.BusinessModel.Account;

public record QueryPagedModelWithKeyword : QueryPagedModel
{
    public string? Keyword { get; init; }

    public void Deconstruct(out int page, out int pageSize, out string sortColumn, out bool orderByDesc, out string? keyword)
    {
        page = Page;
        pageSize = PageSize;
        sortColumn = SortColumn;
        orderByDesc = OrderByDesc;
        keyword = Keyword;
    }
    
    public string GetLikeKeyword()
    {
        return string.IsNullOrEmpty(Keyword) ? string.Empty : $"%{Keyword}%";
    }
}