using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.Payment;
using PhotonPiano.Api.Requests.Tution;
using PhotonPiano.Api.Responses.Payment;
using PhotonPiano.Api.Responses.Tution;
using PhotonPiano.BusinessLogic.BusinessModel.Payment;
using PhotonPiano.BusinessLogic.BusinessModel.Tuition;
using PhotonPiano.BusinessLogic.Interfaces;
using Role = PhotonPiano.DataAccess.Models.Enum.Role;

namespace PhotonPiano.Api.Controllers;

[ApiController]
[Route("api/tuitions")]
public class TuitionController : BaseController
{
    private readonly ILogger<TuitionController> _logger;
    private readonly RedirectUrlValidator _redirectUrlValidator;
    private readonly IServiceFactory _serviceFactory;

    public TuitionController(IServiceFactory serviceFactory, RedirectUrlValidator redirectUrlValidator,
        ILogger<TuitionController> logger)
    {
        _serviceFactory = serviceFactory;
        _redirectUrlValidator = redirectUrlValidator;
        _logger = logger;
    }

    [HttpPost("tuition-fee")]
    [CustomAuthorize(Roles = [Role.Student])]
    public async Task<ActionResult> PayTuitionFee([FromBody] PayTuitionFeeRequest request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

        var apiBaseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";

        var url = await _serviceFactory.TuitionService.PayTuition(CurrentAccount!, request.TuitionId,
            request.ReturnUrl,
            ipAddress, apiBaseUrl);

        return Ok(new PaymentUrlResponse
        {
            Url = url
        }
        );
    }

    [HttpGet("{account-id}/tuition-payment-callback")]
    [EndpointDescription("Enrollment payment callback")]
    public async Task<ActionResult> HandleEnrollmentPaymentCallback(
        [FromQuery] VnPayReturnRequest request,
        [FromRoute(Name = "account-id")] string accountId,
        [FromQuery(Name = "url")] string clientRedirectUrl = "https://default-url.com")
    {
        // Enable this when frontend web have deploy url
        // if (!_redirectUrlValidator.IsValid(clientRedirectUrl))
        // {
        //     _logger.LogWarning("Invalid redirect URL detected: {clientRedirectUrl}", clientRedirectUrl);
        //     return BadRequest(new { Error = "Invalid redirect URL" });
        // }

        await _serviceFactory.TuitionService.HandleTuitionPaymentCallback(
            request.Adapt<VnPayCallbackModel>(), accountId);

        return Redirect(clientRedirectUrl);
    }
 
    [HttpGet]
    [CustomAuthorize(Roles = [Role.Student, Role.Staff])]
    [EndpointDescription("Get Tuition with paging")]
    public async Task<ActionResult<List<TuitionWithStudentClassResponse>>> GetPagedTuitions(
        [FromQuery] QueryTutionRequest request)
    {
        var pagedResult =
            await _serviceFactory.TuitionService.GetTuitionsPaged(request.Adapt<QueryTuitionModel>(),
                CurrentAccount);


        HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);

        return Ok(pagedResult.Items.Adapt<List<TuitionWithStudentClassResponse>>());
    }

    [HttpGet("{id}")]
    [CustomAuthorize(Roles = [Role.Student, Role.Staff])]
    [EndpointDescription("Get Tuition details")]
    public async Task<ActionResult<TuitionWithStudentClassResponse>> GetTuitionDetails(
        [FromRoute] Guid id)
    {
        return Ok(await _serviceFactory.TuitionService.GetTuitionById(id, CurrentAccount));
    }

    // [HttpGet("refund-amount")]
    // [FirebaseAuthorize(Roles = [Role.Student])]
    // [EndpointDescription("Get Refund Tuition Amount")]
    // public async Task<ActionResult> RefundTuitionAmount()
    // {
    //     if (CurrentAccount != null)
    //         return Ok(await _serviceFactory.TuitionService.GetTuitionRefundAmount(CurrentAccount.AccountFirebaseId,
    //             CurrentAccount.CurrentClassId));
    //     return NoContent();
    // }
}