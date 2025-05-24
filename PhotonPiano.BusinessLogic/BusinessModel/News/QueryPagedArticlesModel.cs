using PhotonPiano.BusinessLogic.BusinessModel.Query;

namespace PhotonPiano.BusinessLogic.BusinessModel.News;

public record QueryPagedArticlesModel : QueryPagedModel
{
    public string? Keyword { get; init; }
    
    public bool? IsPublished { get; init; }
    
    public string GetLikeKeyword()
    {
        return string.IsNullOrEmpty(Keyword) ? string.Empty : $"%{Keyword}%";
    }


    public void Deconstruct(out int page, out int pageSize, out string sortColumn, out bool orderByDesc, out string? keyword)
    {
        page = Page;
        pageSize = PageSize;
        sortColumn = SortColumn;
        orderByDesc = OrderByDesc;
        keyword = Keyword;
    }
}