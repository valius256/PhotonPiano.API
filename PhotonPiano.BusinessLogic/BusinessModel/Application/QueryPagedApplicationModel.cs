using PhotonPiano.BusinessLogic.BusinessModel.Query;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Application;

public record QueryPagedApplicationModel : QueryPagedModel
{
    public string? Keyword { get; init; }

    public List<ApplicationType> Types { get; init; } = [];

    public List<ApplicationStatus> Statuses { get; init; } = [];

    public void Deconstruct(out int page, out int pageSize, out string sortColumn, out bool orderByDesc, out string? keyword, out List<ApplicationType> types, out List<ApplicationStatus> statuses)
    {
        page = Page;
        pageSize = PageSize;
        sortColumn = SortColumn;
        orderByDesc = OrderByDesc;
        keyword = Keyword;
        types = Types;
        statuses = Statuses;
    }
}