namespace PhotonPiano.Api.Requests.Account;

public record UpdateContinuationStatusRequest
{
    public bool WantToContinue { get; init; }
}