﻿using PhotonPiano.BusinessLogic.BusinessModel.Query;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Account;

public record QueryPagedAccountsModel : QueryPagedModel
{
    public string? Keyword { get; init; }
    public List<Role> Roles { get; init; } = [];
    public List<Guid> Levels { get; init; } = [];
    public List<AccountStatus> Statuses { get; init; } = [];
    public List<StudentStatus> StudentStatuses { get; init; } = [];
    public List<TuitionStatus> TuitionStatuses { get; init; } = [];
    
    public string GetLikeKeyword()
    {
        return string.IsNullOrEmpty(Keyword) ? string.Empty : $"%{Keyword}%";
    }
    public void Deconstruct(out int page, out int pageSize, out string sortColumn, out bool orderByDesc)
    {
        page = Page;
        pageSize = PageSize;
        sortColumn = SortColumn;
        orderByDesc = OrderByDesc;
    }

    public void Deconstruct(out int page, out int pageSize, out string sortColumn, out bool orderByDesc, out string? keyword, out List<Role> roles,
        out List<Guid> levels, out List<StudentStatus> studentStatuses, out List<AccountStatus> accountStatuses, out List<TuitionStatus> tuitionStatuses)
    {
        page = Page;
        pageSize = PageSize;
        sortColumn = SortColumn;
        orderByDesc = OrderByDesc;
        keyword = Keyword;
        roles = Roles;
        levels = Levels;
        studentStatuses = StudentStatuses;
        accountStatuses = Statuses;
        tuitionStatuses = TuitionStatuses;
    }
}