using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.Auth;

public record SignInRequest(
    [Required] [EmailAddress] string Email,
    [Required] string Password
);