using Microsoft.AspNetCore.Mvc;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.Shared.Models;
using System.Security.Claims;

namespace PhotonPiano.Api.Controllers;

public class BaseController : Controller
{
    // protected readonly ILogger<T> _logger;
    //
    // public BaseController(ILogger<T> logger)
    // {
    //     _logger = logger;
    // }


    protected string CurrentUserFirebaseId
    {
        get
        {
            var firebaseId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(firebaseId)) return firebaseId;

            // _logger.LogInformation("Can't get user firebaseId from HttpContext");

            return string.Empty;
        }
    }

    protected AccountModel? CurrentAccount
    {
        get
        {
            HttpContext.Items.TryGetValue("Account", out var account);

            return account as AccountModel;
        }
    }

    protected string CurrentUserId
    {
        get
        {
            var firebaseId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userEntityId")?.Value;
            if (firebaseId is not null) return firebaseId;

            // _logger.LogInformation("Can't get userEntityId from HttpContext");
            return string.Empty;
        }
    }

    protected async Task<IApiResult<F>> OkAsync<F>(Task<F> action, string? op = null)
    {
        var result = await Task.Run(() => action);

        return new ApiResult<F>
        {
            Op = op,
            Status = "OK",
            Data = result
        };
    }

    protected static IApiResult<F> OkAsync<F>(F data, string? op = null)
    {
        return new ApiResult<F>
        {
            Op = op,
            Status = "OK",
            Data = data
        };
    }
}