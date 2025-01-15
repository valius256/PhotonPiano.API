namespace PhotonPiano.BusinessLogic.BusinessModel.Query;

public record QueryPagedModel
{
    public int Page { get; init; }
    public int PageSize { get; init; }

    public required string SortColumn { get; init; }

    public bool OrderByDesc { get; init; }

    public void Deconstruct(out int page, out int pageSize, out string sortColumn, out bool orderByDesc)
    {
        page = Page;
        pageSize = PageSize;
        sortColumn = SortColumn;
        orderByDesc = OrderByDesc;
    }
}