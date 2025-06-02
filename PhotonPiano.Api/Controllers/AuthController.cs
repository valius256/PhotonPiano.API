using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Requests.Account;
using PhotonPiano.Api.Requests.Auth;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Auth;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : BaseController
{
    private readonly IServiceFactory _serviceFactory;

    public AuthController(IServiceFactory serviceFactory)
    {
        _serviceFactory = serviceFactory;
    }

    [HttpPost("sign-in")]
    [EndpointDescription("Sign in with email and password")]
    public async Task<ActionResult<AuthModel>> SignIn([FromBody] SignInRequest request)
    {
        var (email, password) = request;
        return await _serviceFactory.AuthService.SignIn(email, password);
    }

    [HttpPost("sign-up")]
    [EndpointDescription("Sign up student account")]
    public async Task<ActionResult<AccountModel>> SignUp([FromBody] SignUpRequest request)
    {
        return Created(nameof(SignUp), await _serviceFactory.AuthService.SignUp(request.Adapt<SignUpModel>()));
    }

    [HttpGet("current-info")]
    [CustomAuthorize]
    [EndpointDescription("Get current account info")]
    public ActionResult<AccountModel?> GetCurrentAccountInfo()
    {
        return CurrentAccount is null || CurrentAccount.Status == AccountStatus.Inactive
            ? Unauthorized("Unauthorized")
            : CurrentAccount;
    }

    [HttpPost("token-refresh")]
    [EndpointDescription("Refresh id token")]
    public async Task<ActionResult<NewIdTokenModel>> RefreshToken([FromBody] RefreshIdTokenRequest request)
    {
        return await _serviceFactory.AuthService.RefreshToken(request.RefreshToken);
    }

    [HttpPost("password-reset-email")]
    [EndpointDescription("Send password reset email")]
    public async Task<ActionResult> SendPasswordResetEmail([FromBody] SendPasswordResetEmailRequest request)
    {
        await _serviceFactory.AuthService.SendPasswordResetEmail(request.Email);
        return Ok();
    }

    [HttpGet("google-auth-callback")]
    [EndpointDescription("Google Auth callback")]
    public async Task<ActionResult<OAuthCredentialsModel>> HandleGoogleAuthCallback(
        [FromQuery] GoogleAuthCallbackRequest request)
    {
        var (code, redirectUrl) = request;
        return await _serviceFactory.AuthService.HandleGoogleAuthCallback(code, redirectUrl); 
    }

    [HttpPut("change-password")]
    [EndpointDescription("Change password")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        await _serviceFactory.AuthService.ChangePassword(request.Adapt<ChangePasswordModel>());
        return NoContent();
    }

    [HttpPost("toggle-account-status")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Administrator])]
    [EndpointDescription("Toggle account status")]
    public async Task<ActionResult> ToggleAccountStatus([FromBody] ToggleAccountStatusRequest request)
    {
        await _serviceFactory.AuthService.ToggleAccountStatus(request.FirebaseUid);
        return NoContent();
    }
}