using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Requests.Query;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.Account;

public record QueryPagedAccountsRequest : QueryPagedRequest
{
    [FromQuery(Name = "q")] public string? Keyword { get; init; }

    [FromQuery(Name = "levels")] public List<LevelEnum> Levels { get; init; } = [];

    [FromQuery(Name = "student-statuses")] public List<StudentStatus> StudentStatuses { get; init; } = [];

    [FromQuery(Name = "roles")] public List<Role> Roles { get; init; } = [];
}