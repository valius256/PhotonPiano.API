using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.Account;
using PhotonPiano.Api.Requests.Auth;
using PhotonPiano.Api.Responses.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Auth;
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
    [CustomAuthorize(Roles = [Role.Staff, Role.Administrator, Role.Instructor])]
    [EndpointDescription("Get accounts with paging")]
    public async Task<ActionResult<List<AccountModel>>> GetAccounts([FromQuery] QueryPagedAccountsRequest request)
    {
        var pagedResult =
            await _serviceFactory.AccountService.GetAccounts(base.CurrentAccount!,
                request.Adapt<QueryPagedAccountsModel>());

        HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);

        return pagedResult.Items;
    }

    [HttpGet("teachers")]
    [EndpointDescription("Get teachers with paging")]
    public async Task<ActionResult<List<AccountModel>>> GetTeachers([FromQuery] QueryPagedAccountsRequest request)
    {
        var pagedResult =
            await _serviceFactory.AccountService.GetTeachers(request.Adapt<QueryPagedAccountsModel>());

        HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);
        return pagedResult.Items;
    }

    [HttpGet("class-waiting")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Administrator])]
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

    [HttpGet("{id}")]
    [CustomAuthorize]
    [EndpointDescription("Get Account details by id")]
    public async Task<ActionResult<AccountDetailResponse>> GetAccountById(
        [FromRoute] string id)
    {
        var result = await _serviceFactory.AccountService.GetAccountById(id);
        
        return result.Adapt<AccountDetailResponse>();
    }

    [HttpGet("{id}/teacher")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Administrator,Role.Instructor, Role.Student])]
    [EndpointDescription("Get teacher detail by id")]
    public async Task<ActionResult<TeacherDetailsResponse>> GetTeacherDetailById(
        [FromRoute] string id)
    {
        var result = await _serviceFactory.AccountService.GetTeacherDetailById(id);

        return result.Adapt<TeacherDetailsResponse>();
    }

    [HttpPut]
    [CustomAuthorize]
    [EndpointDescription("Update account info")]
    public async Task<ActionResult> UpdateAccountInfo([FromBody] UpdateAccountRequest request)
    {
        await _serviceFactory.AccountService.UpdateAccount(request.Adapt<UpdateAccountModel>(),
            base.CurrentAccount!);

        return NoContent();
    }

    [HttpPost("staff")]
    [CustomAuthorize(Roles = [Role.Administrator])]
    [EndpointDescription("Create new staff")]
    public async Task<ActionResult> CreateNewStaff([FromBody] CreateSystemAccountRequest request)
    {
        var result = await _serviceFactory.AccountService.CreateNewStaff(request.Adapt<CreateSystemAccountModel>());
        return Created(nameof(CreateNewStaff), result);
    }
    [HttpPost("teacher")]
    [CustomAuthorize(Roles = [Role.Administrator])]
    [EndpointDescription("Create new staff")]
    public async Task<ActionResult> CreateNewTeacher([FromBody] CreateSystemAccountRequest request)
    {
        var result = await _serviceFactory.AccountService.CreateNewTeacher(request.Adapt<CreateSystemAccountModel>());
        return Created(nameof(CreateNewTeacher), result);
    }

    [HttpPut("role")]
    [CustomAuthorize(Roles = [Role.Administrator, Role.Staff /*for testing production*/])]
    [EndpointDescription("Change Role")]
    public async Task<ActionResult> ChangeRole([FromBody] ChangeRoleRequest request)
    {
        await _serviceFactory.AccountService.ChangeRole(request.Adapt<GrantRoleModel>());
        return NoContent();
    }

    [HttpPut("continuation-status")]
    [CustomAuthorize(Roles = [Role.Student, Role.Staff, Role.Instructor])]
    [EndpointDescription("Update student continuation status")]
    public async Task<IActionResult> UpdateContinuationStatus([FromBody] UpdateContinuationStatusRequest request)
    {
        var result = await _serviceFactory.AccountService.UpdateContinuingLearningStatus(
            CurrentAccount!.AccountFirebaseId, 
            request.WantToContinue);
        return Ok(result);
    }


    //[HttpGet("{account-id}/attempt-stats")]
    //[FirebaseAuthorize]
    //public async Task<ActionResult> GetAttemptsStats([FromRoute(UserName = "account-id")] string accountId)
    //{
    //    return Ok();
    //}
}