using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.Auth
{
    public record ChangePasswordRequest
    {
        public required string Password { get; init; }
        [EmailAddress]
        public required string Email { get; init; }
        public required string ResetPasswordToken { get; init; }
    }
}
