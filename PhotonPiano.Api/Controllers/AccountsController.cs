using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Requests.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.Interfaces;

namespace PhotonPiano.Api.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountsController : BaseController
{
    private readonly IServiceFactory _serviceFactory;

    public AccountsController(IServiceFactory serviceFactory)
    {
        _serviceFactory = serviceFactory;
    }

    [HttpGet]
    [EndpointDescription("Get accounts")]
    public async Task<ActionResult<List<AccountModel>>> GetAccounts()
    {
        return await _serviceFactory.AccountService.GetAccounts();
    }

    [HttpPost]
    [EndpointDescription("Create account")]
    public async Task<ActionResult<AccountModel>> CreateAccount([FromBody] CreateAccountRequest request)
    {
        var result = await _serviceFactory.AccountService.CreateAccount(request.FirebaseUid, request.Email);

        return Created(nameof(CreateAccount), result);
    }
    
    [HttpGet("{accountFirebaseId}")]
    [EndpointDescription("Get Account details by id")]
    public async Task<ActionResult<AccountModel>> GetAccountById([FromRoute] string accountFirebaseId)
    {
        var result = await _serviceFactory.AccountService.GetAccountById(accountFirebaseId);

        return Ok(result);
    }

    //[HttpGet("{account-id}/attempt-stats")]
    //[FirebaseAuthorize]
    //public async Task<ActionResult> GetAttemptsStats([FromRoute(Name = "account-id")] string accountId)
    //{
    //    return Ok();
    //}
}