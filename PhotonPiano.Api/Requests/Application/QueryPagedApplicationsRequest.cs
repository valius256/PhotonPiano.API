using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Requests.Query;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.Application;

public record QueryPagedApplicationsRequest : QueryPagedRequest
{
    [FromQuery(Name = "q")]
    public string? Keyword { get; init; }

    [FromQuery(Name = "types")] 
    public List<ApplicationType> Types { get; init; } = [];
    
    [FromQuery(Name = "statuses")]
    public List<ApplicationStatus> Statuses { get; init; } = [];
}