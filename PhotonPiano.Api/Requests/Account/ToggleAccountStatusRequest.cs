namespace PhotonPiano.Api.Requests.Account;

public record ToggleAccountStatusRequest
{
    public required string FirebaseUid { get; init; }
}