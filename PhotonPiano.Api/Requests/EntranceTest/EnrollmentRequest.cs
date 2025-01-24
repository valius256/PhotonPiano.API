using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.EntranceTest;

public record EnrollmentRequest(
    [Required] string ReturnUrl
);