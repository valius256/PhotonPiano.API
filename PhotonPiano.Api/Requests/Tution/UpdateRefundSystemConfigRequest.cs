using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.Tution;

public record UpdateRefundSystemConfigRequest
{
    [MinLength(1, ErrorMessage = "At least one reason for refund tuition is required")]
    [Required(ErrorMessage = "Reasons for refund tuition are required")]
    public List<string> ReasonRefundTuition { get; init; } = new();
}