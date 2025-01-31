using PhotonPiano.BusinessLogic.BusinessModel.Query;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Account;

public record QueryPagedAccountsModel : QueryPagedModel
{
    public string? Keyword { get; init; }
    public List<Role> Roles { get; init; } = [];
    public List<Level> Levels { get; init; } = [];
    public List<StudentStatus> StudentStatuses { get; init; } = [];

    public void Deconstruct(out int page, out int pageSize, out string sortColumn, out bool orderByDesc)
    {
        page = Page;
        pageSize = PageSize;
        sortColumn = SortColumn;
        orderByDesc = OrderByDesc;
    }

    public void Deconstruct(out int page, out int pageSize, out string sortColumn, out bool orderByDesc, out string? keyword, out List<Role> roles, 
        out List<Level> levels, out List<StudentStatus> studentStatuses)
    {
        page = Page;
        pageSize = PageSize;
        sortColumn = SortColumn;
        orderByDesc = OrderByDesc;
        keyword = Keyword;
        roles = Roles;
        levels = Levels;
        studentStatuses = StudentStatuses;
    }
}