using Microsoft.AspNetCore.Mvc;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
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

            if (!string.IsNullOrEmpty(firebaseId))
            {
                return firebaseId;
            }

            // _logger.LogInformation("Can't get user firebaseId from HttpContext");

            return string.Empty;
        }
    }

    protected AccountModel? CurrentAccount
    {
        get
        {
            HttpContext.Items.TryGetValue(key: "Account", value: out var account);

            return account as AccountModel;
        }
    }

    protected string CurrentUserId
    {
        get
        {
            var firebaseId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userEntityId")?.Value;
            if (firebaseId is not null)
            {
                return firebaseId;
            }

            // _logger.LogInformation("Can't get userEntityId from HttpContext");
            return string.Empty;
        }
    }
}