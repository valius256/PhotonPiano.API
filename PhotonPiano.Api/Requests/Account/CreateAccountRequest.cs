namespace PhotonPiano.Api.Requests.Account;

public class CreateAccountRequest
{
    public required string FirebaseUid { get; init; }
    public required string Email { get; init; }
}