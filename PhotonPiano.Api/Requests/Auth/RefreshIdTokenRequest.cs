using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.Auth;

public record RefreshIdTokenRequest(
    [Required] string RefreshToken);