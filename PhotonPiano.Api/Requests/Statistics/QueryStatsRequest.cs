using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace PhotonPiano.Api.Requests.Statistics;

public record QueryStatsRequest
{
    [FromQuery(Name = "month")] 
    [Range(1, 12, ErrorMessage = "Month must be between 1 and 12")]
    public int? Month { get; init; }
    
    [FromQuery(Name = "year")] 
    public int? Year { get; init; } 
}