using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.Auth;

public record SignUpRequest(
    [Required] [EmailAddress] string Email,
    [Required] string Password
);