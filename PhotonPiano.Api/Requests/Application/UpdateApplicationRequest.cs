using System.ComponentModel.DataAnnotations;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.Application;

public record UpdateApplicationRequest
{
    [Required(ErrorMessage = "Status is required")]
    [EnumDataType(typeof(ApplicationStatus), ErrorMessage = "Invalid status")]
    public required ApplicationStatus Status { get; init; }

    public string? Note { get; init; }
}