namespace PhotonPiano.Api.Requests.Tution;

public record PayTuitionFeeRequest
{
    public string ReturnUrl { get; init; }

    public Guid TutionId { get; init; }
}