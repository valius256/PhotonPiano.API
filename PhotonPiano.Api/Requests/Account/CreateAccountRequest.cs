namespace PhotonPiano.Api.Requests.Account;

public class CreateAccountRequest
{
    public required string FirebaseUid { get; init; }
    public string Email { get; init; }
}