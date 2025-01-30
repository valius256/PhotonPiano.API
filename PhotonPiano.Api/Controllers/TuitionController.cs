using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.Payment;
using PhotonPiano.Api.Requests.Tution;
using PhotonPiano.Api.Responses.Payment;
using PhotonPiano.BusinessLogic.BusinessModel.Payment;
using PhotonPiano.BusinessLogic.Interfaces;
using Role = PhotonPiano.DataAccess.Models.Enum.Role;

namespace PhotonPiano.Api.Controllers;

[ApiController]
[Route("api/tutions")]
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

    [HttpPost("tution-fee")]
    [FirebaseAuthorize(Roles = [Role.Student])]
    public async Task<ActionResult> PayTuitionFee([FromBody] PayTuitionFeeRequest request)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

        var apiBaseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";

        return Ok(new PaymentUrlResponse
            {
                Url = await _serviceFactory.TutionService.PayTuition(CurrentUserFirebaseId, request.TutionId,
                    request.ReturnUrl,
                    ipAddress, apiBaseUrl)
            }
        );
    }

    [HttpGet("{account-id}/tution-payment-callback")]
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

        await _serviceFactory.TutionService.HandleTutionPaymentCallback(
            request.Adapt<VnPayCallbackModel>(), accountId);

        return Redirect(clientRedirectUrl);
    }
}