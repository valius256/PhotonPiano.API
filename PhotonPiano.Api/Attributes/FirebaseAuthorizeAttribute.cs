using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Exceptions;
using System.Security.Claims;

namespace PhotonPiano.Api.Attributes;

public class FirebaseAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
{
    public new Role[] Roles { get; set; } = [];

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        var firebaseId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(firebaseId))
        {
            context.Result = new UnauthorizedResult();

            throw new ForbiddenMethodException("You don't have permission to access this resource");
        }

        var email = user.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email)) throw new UnauthorizedException("Invalid email in claims");

        var isEmailVerified = user.FindFirst(c => c.Type == "email_verified")?.Value == "true";

        var accountService = context.HttpContext.RequestServices.GetRequiredService<IAccountService>();

        var account =
            await accountService.GetAndCreateAccountIfNotExistsCredentials(firebaseId, email, isEmailVerified);

        context.HttpContext.Items["Account"] = account;

        if (Roles is []) return;

        if (!Roles.Contains(account.Role))
            throw new ForbiddenMethodException("You don't have permission to access this resource");
    }
}