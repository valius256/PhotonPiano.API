using System.Security.Claims;
using PhotonPiano.BusinessLogic.BusinessModel.Account;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ITokenService
{
    string GenerateIdToken(AccountModel account);

    string GenerateRefreshToken();

    ClaimsPrincipal GetClaimsPrincipalFromExpiredToken(string expiredIdToken);
}