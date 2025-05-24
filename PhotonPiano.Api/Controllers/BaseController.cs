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

    protected string CurrentAccountId
    {
        get
        {
            var accountId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return !string.IsNullOrEmpty(accountId) ? accountId :
                string.Empty;
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