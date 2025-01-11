using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace L2Drive.API.Requests.Query;

public record QueryPagedRequest
{
    [FromQuery(Name = "page")]
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than zero.")]
    public int Page { get; init; } = 1;
 
    [FromQuery(Name = "size")]
    [Range(1, int.MaxValue, ErrorMessage = "Page size must be greater than zero.")]
    public int PageSize { get; init; } = 10;
 
    [FromQuery(Name = "column")]
    public string SortColumn { get; init; } = "Id";

    [FromQuery(Name = "desc")] 
    public bool OrderByDesc { get; init; } = true;
}