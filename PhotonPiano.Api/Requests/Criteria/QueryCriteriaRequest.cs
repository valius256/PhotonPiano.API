using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Requests.Query;

namespace PhotonPiano.Api.Requests.Criteria;

public record QueryCriteriaRequest : QueryPagedRequest
{
    [FromQuery(Name = "keyword")]
    public string? Keyword { get; set; }
    
    
}