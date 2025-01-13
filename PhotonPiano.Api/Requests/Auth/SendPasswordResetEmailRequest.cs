using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.Auth;

public record SendPasswordResetEmailRequest(
    [Required] [EmailAddress] string Email
);