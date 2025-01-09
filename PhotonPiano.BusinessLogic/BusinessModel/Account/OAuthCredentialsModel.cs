namespace PhotonPiano.BusinessLogic.BusinessModel.Account;

public record OAuthCredentialsModel
{
    public required string LocalId { get; init; }

    public bool EmailVerified { get; init; }

    public required string Email { get; init; }

    public required string IdToken { get; init; }

    public required string RefreshToken { get; init; }

    public required string ExpiresIn { get; init; }
    
    public required string Role { get; set; } = string.Empty;
    
    public void Deconstruct(out string localId, out bool emailVerified, out string email, out string idToken, out string refreshToken, out string expiresIn, out string role)
    {
        localId = LocalId;
        emailVerified = EmailVerified;
        email = Email;
        idToken = IdToken;
        refreshToken = RefreshToken;
        expiresIn = ExpiresIn;
        role = Role;
    }
}