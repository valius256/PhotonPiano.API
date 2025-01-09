using PhotonPiano.BusinessLogic.BusinessModel.Account;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IAuthService
{
    Task<AuthModel> SignIn(string email, string password);

    Task<AccountModel> SignUp(string email, string password);

    Task<NewIdTokenModel> RefreshToken(string refreshToken);

    Task SendPasswordResetEmail(string email);
    Task<OAuthCredentialsModel> HandleGoogleAuthCallback(string code, string redirectUrl);
}