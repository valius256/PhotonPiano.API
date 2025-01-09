namespace PhotonPiano.BusinessLogic.BusinessModel.Auth;

public record GoogleAuthModel
{
    public required string AccessToken { get; init; }

    public int ExpiresIn { get; init; }
    public required string IdToken { get; init; }
}