using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Auth;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IAuthService
{
    Task<AuthModel> SignIn(string email, string password);
    
    Task<AuthModel> SignUp(SignUpModel model);
    
    Task<NewIdTokenModel> RefreshToken(string refreshToken);

    Task SendPasswordResetEmail(string email);
    Task<OAuthCredentialsModel> HandleGoogleAuthCallback(string code, string redirectUrl);
    Task<string> SignUpOnFirebase(string email, string password);
    Task UpdateFirebaseEmail(string idToken, string newEmail);
    Task ChangePassword(ChangePasswordModel changePasswordModel);
}