using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace PhotonPiano.Api.Requests.EntranceTest;

public record RemoveStudentsFromEntranceTestRequest
{
    [Required]
    [FromQuery(Name = "studentIds")]
    public required List<string> StudentIds { get; init; }
}