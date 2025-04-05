using Microsoft.AspNetCore.Mvc;
using PhotonPiano.DataAccess.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.Application;

public record SendApplicationRequest
{
    [Required(ErrorMessage = "Application type is required")]
    [FromForm(Name = "type")]
    public required ApplicationType Type { get; init; }

    [FromForm(Name = "reason")]
    [Required(ErrorMessage = "Reason is required")]
    public required string Reason { get; init; }

    // [AllowNull]
    [FromForm(Name = "file")]
    public IFormFile? File { get; init; }
}