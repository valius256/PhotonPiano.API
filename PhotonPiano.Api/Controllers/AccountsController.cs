using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.Account;
using PhotonPiano.Api.Responses.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Enum;

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
    [FirebaseAuthorize(Roles = [Role.Staff, Role.Administrator, Role.Instructor])]
    [EndpointDescription("Get accounts with paging")]
    public async Task<ActionResult<List<AccountModel>>> GetAccounts([FromQuery] QueryPagedAccountsRequest request)
    {
        var pagedResult =
            await _serviceFactory.AccountService.GetAccounts(base.CurrentAccount!,
                request.Adapt<QueryPagedAccountsModel>());

        HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);

        return pagedResult.Items;
    }

    [HttpGet("class-waiting")]
    [FirebaseAuthorize(Roles = [Role.Staff, Role.Administrator])]
    [EndpointDescription("Get accounts with paging")]
    public async Task<ActionResult<List<AwaitingLevelsModel>>> GetWaitingStudentOfAllLevels()
    {
        return await _serviceFactory.AccountService.GetWaitingStudentOfAllLevels();
    }

    [HttpPost]
    [EndpointDescription("Create account")]
    public async Task<ActionResult<AccountModel>> CreateAccount([FromBody] CreateAccountRequest request)
    {
        var result = await _serviceFactory.AccountService.CreateAccount(request.FirebaseUid, request.Email);

        return Created(nameof(CreateAccount), result);
    }

    [HttpGet("{firebase-id}")]
    [EndpointDescription("Get Account details by id")]
    public async Task<ActionResult<AccountDetailResponse>> GetAccountById(
        [FromRoute(Name = "firebase-id")] string accountFirebaseId)
    {
        var result = await _serviceFactory.AccountService.GetAccountById(accountFirebaseId);

        return Ok(result.Adapt<AccountDetailResponse>());
    }

    [HttpPut]
    [FirebaseAuthorize]
    [EndpointDescription("Update account info")]
    public async Task<ActionResult> UpdateAccountInfo([FromBody] UpdateAccountRequest request,
        [FromHeader(Name = "Authorization")] string bearerToken)
    {
        string idToken = bearerToken.Replace("Bearer ", "");
        await _serviceFactory.AccountService.UpdateAccount(request.Adapt<UpdateAccountModel>(),
            base.CurrentAccount!, idToken);

        return NoContent();
    }

    //[HttpGet("{account-id}/attempt-stats")]
    //[FirebaseAuthorize]
    //public async Task<ActionResult> GetAttemptsStats([FromRoute(UserName = "account-id")] string accountId)
    //{
    //    return Ok();
    //}
}