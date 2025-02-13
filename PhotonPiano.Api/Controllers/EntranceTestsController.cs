using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.EntranceTest;
using PhotonPiano.Api.Requests.Payment;
using PhotonPiano.Api.Requests.Query;
using PhotonPiano.Api.Responses.EntranceTest;
using PhotonPiano.Api.Responses.Payment;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;
using PhotonPiano.BusinessLogic.BusinessModel.Payment;
using PhotonPiano.BusinessLogic.BusinessModel.Query;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Controllers;

[ApiController]
[Route("api/entrance-tests")]
public class EntranceTestsController : BaseController
{
    private readonly IServiceFactory _serviceFactory;

    public EntranceTestsController(IServiceFactory serviceFactory)
    {
        _serviceFactory = serviceFactory;
    }

    [HttpGet]
    [FirebaseAuthorize(Roles = [Role.Student, Role.Staff, Role.Instructor])]
    [EndpointDescription(
        "Get entrance tests with paging, field Keyword can search for EntranceTestName, InstructorName, RoomName")]
    public async Task<ActionResult<List<EntranceTestResponse>>> GetEntranceTest(
        [FromQuery] QueryEntranceTestRequest request)
    {
        var pagedResult =
            await _serviceFactory.EntranceTestService.GetPagedEntranceTest(
                request.Adapt<QueryEntranceTestModel>(), base.CurrentAccount!);

        HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);

        return pagedResult.Items.Adapt<List<EntranceTestResponse>>();
    }

    [HttpGet("{id}")]
    [FirebaseAuthorize(Roles = [Role.Student, Role.Staff, Role.Instructor])]
    [EndpointDescription("Get an entrance test")]
    public async Task<ActionResult<EntranceTestDetailResponse>> GetEntranceTestById([FromRoute] Guid id)
    {
        var result = await _serviceFactory.EntranceTestService.GetEntranceTestDetailById(id, base.CurrentAccount!);
        return result.Adapt<EntranceTestDetailResponse>();
    }

    [HttpPost]
    [FirebaseAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Create an entrance test")]
    public async Task<ActionResult<EntranceTestResponse>> CreateEntranceTest(
        [FromBody] CreateEntranceTestRequest request)
    {
        var result =
            await _serviceFactory.EntranceTestService.CreateEntranceTest(
                request.Adapt<CreateEntranceTestModel>(), base.CurrentAccount!);
        return Created(nameof(CreateEntranceTest), result.Adapt<EntranceTestResponse>());
    }

    [HttpDelete("{id}")]
    [FirebaseAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Delete an entrance test")]
    public async Task<ActionResult> DeleteEntranceTest([FromRoute] Guid id)
    {
        await _serviceFactory.EntranceTestService.DeleteEntranceTest(id, CurrentUserFirebaseId);
        return NoContent();
    }

    [HttpPut("{id}")]
    [FirebaseAuthorize(Roles = [Role.Staff, Role.Instructor])]
    [EndpointDescription("Update an entrance test")]
    public async Task<ActionResult> UpdateEntranceTest([FromRoute] Guid id,
        [FromBody] UpdateEntranceTestRequest request)
    {
        await _serviceFactory.EntranceTestService.UpdateEntranceTest(id, request.Adapt<UpdateEntranceTestModel>(),
            CurrentUserFirebaseId);

        return NoContent();
    }

    [HttpGet("{id}/students")]
    [FirebaseAuthorize(Roles = [Role.Staff, Role.Student])]
    [EndpointDescription("Get entrance tests students")]
    public async Task<ActionResult<List<EntranceTestStudentDetail>>> GetEntranceTestStudents([FromRoute] Guid id,
        [FromQuery] QueryPagedRequest query)
    {
        var pagedResult = await _serviceFactory.EntranceTestService.GetPagedEntranceTestStudent(
            query.Adapt<QueryPagedModel>(), id,
            base.CurrentAccount!);

        HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);

        return pagedResult.Items;
    }

    [HttpGet("{id}/students/{student-id}")]
    [FirebaseAuthorize(Roles = [Role.Staff, Role.Student])]
    [EndpointDescription("Get entrance test student details")]
    public async Task<ActionResult<EntranceTestStudentDetail>> GetEntranceTestStudentDetails(
        [FromRoute(Name = "id")] Guid id,
        [FromRoute(Name = "student-id")] string studentId)
    {
        return await _serviceFactory.EntranceTestService.GetEntranceTestStudentDetail(id, studentId,
            base.CurrentAccount!);
    }

    [HttpPost("enrollment-requests")]
    [FirebaseAuthorize(Roles = [Role.Student])]
    [EndpointDescription("Enroll student in entrance test")]
    public async Task<ActionResult<PaymentUrlResponse>> EnrollEntranceTest([FromBody] EnrollmentRequest request)
    {
        string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

        string apiBaseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";

        return Created(nameof(EnrollEntranceTest),
            new PaymentUrlResponse
            {
                Url = await _serviceFactory.EntranceTestService.EnrollEntranceTest(base.CurrentAccount!,
                    request.ReturnUrl,
                    ipAddress, apiBaseUrl)
            }
        );
    }

    [HttpGet("{account-id}/enrollment-payment-callback")]
    [EndpointDescription("Enrollment payment callback")]
    public async Task<ActionResult> HandleEnrollmentPaymentCallback([FromQuery] VnPayReturnRequest request,
        [FromRoute(Name = "account-id")] string accountId,
        [FromQuery(Name = "url")] string clientRedirectUrl)
    {
        await _serviceFactory.EntranceTestService.HandleEnrollmentPaymentCallback(
            request.Adapt<VnPayCallbackModel>(), accountId);

        return Redirect(clientRedirectUrl);
    }

    [HttpPost("auto-arrangement")]
    [EndpointDescription("Auto arrange entrance tests")]
    [FirebaseAuthorize(Roles = [Role.Staff])]
    public async Task<ActionResult<List<EntranceTestDetailResponse>>> AutoArrangeEntranceTests(
        [FromBody] AutoArrangeEntranceTestsRequest request)
    {
        var result =
            await _serviceFactory.EntranceTestService.AutoArrangeEntranceTests(
                request.Adapt<AutoArrangeEntranceTestsModel>(), base.CurrentAccount!);

        return Created(nameof(AutoArrangeEntranceTests), result.Adapt<List<EntranceTestDetailResponse>>());
    }
}