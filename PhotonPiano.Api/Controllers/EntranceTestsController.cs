using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.EntranceTest;
using PhotonPiano.Api.Requests.Payment;
using PhotonPiano.Api.Requests.Query;
using PhotonPiano.Api.Responses.EntranceTest;
using PhotonPiano.Api.Responses.Payment;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestResult;
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
    [CustomAuthorize(Roles = [Role.Student, Role.Staff, Role.Instructor])]
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
    [CustomAuthorize(Roles = [Role.Student, Role.Staff, Role.Instructor])]
    [EndpointDescription("Get an entrance test")]
    public async Task<ActionResult<EntranceTestDetailResponse>> GetEntranceTestById([FromRoute] Guid id)
    {
        var entranceTest =
            await _serviceFactory.EntranceTestService.GetEntranceTestDetailById(id, base.CurrentAccount!);

        return entranceTest.Adapt<EntranceTestDetailResponse>();
    }

    [HttpGet("{id}/available-teachers")]
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Get all available teachers for entrance test")]
    public async Task<ActionResult<List<AccountModel>>> GetAvailableTeachersForTest([FromRoute] Guid id,
        [FromQuery] QueryPagedRequestWithKeyword request)
    {
        var pagedResults = await _serviceFactory.EntranceTestService.GetPagedAvailableTeachersForTest(
            request.Adapt<QueryPagedModelWithKeyword>(), id, base.CurrentAccount!);
        
        HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResults);

        return pagedResults.Items;
    }

    [HttpPost]
    [CustomAuthorize(Roles = [Role.Staff])]
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
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Delete an entrance test")]
    public async Task<ActionResult> DeleteEntranceTest([FromRoute] Guid id)
    {
        await _serviceFactory.EntranceTestService.DeleteEntranceTest(id, CurrentAccountId);
        return NoContent();
    }

    [HttpPut("{id}")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Instructor])]
    [EndpointDescription("Update an entrance test")]
    public async Task<ActionResult> UpdateEntranceTest([FromRoute] Guid id,
        [FromBody] UpdateEntranceTestRequest request)
    {
        await _serviceFactory.EntranceTestService.UpdateEntranceTest(id, request.Adapt<UpdateEntranceTestModel>(),
            CurrentAccountId);

        return NoContent();
    }

    [HttpPut("{id}/score-announcement-status")]
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Update an entrance test score announcement status")]
    public async Task<ActionResult> UpdateEntranceTestScoreAnnouncedStatus([FromRoute] Guid id,
        [FromBody] UpdateEntranceTestScoreAnnouncementRequest request)
    {
        await _serviceFactory.EntranceTestService.UpdateEntranceTestScoreAnnouncementStatus(id,
            request.IsAnnounced, base.CurrentAccount!);

        return NoContent();
    }

    [HttpGet("{id}/students")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Student])]
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

    [HttpPost("{id}/students")]
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Add student to entrance test")]
    public async Task<ActionResult> AddStudentsToEntranceTest(
        [FromRoute] Guid id,
        [FromBody] AddStudentsToEntranceTestRequest request)
    {
        await _serviceFactory.EntranceTestService.AddStudentsToEntranceTest(id,
            request.Adapt<AddStudentsToEntranceTestModel>(),
            base.CurrentAccount!);

        return Created();
    }

    [HttpDelete("{id}/students")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Student])]
    [EndpointDescription("Remove students from a test")]
    public async Task<ActionResult> RemoveStudentsFromTest([FromRoute(Name = "id")] Guid id,
        [FromQuery] RemoveStudentsFromEntranceTestRequest request)
    {
        await _serviceFactory.EntranceTestService.RemoveStudentsFromTest(id, base.CurrentAccount!, request.StudentIds);

        return NoContent();
    }

    [HttpGet("{id}/students/{student-id}")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Student])]
    [EndpointDescription("Get entrance test student details")]
    public async Task<ActionResult<EntranceTestStudentDetailResponse>> GetEntranceTestStudentDetails(
        [FromRoute(Name = "id")] Guid id,
        [FromRoute(Name = "student-id")] string studentId)
    {
        var (theoryPercentage, practicalPercentage) =
            await _serviceFactory.EntranceTestService.GetScorePercentagesAsync();

        HttpContext.Response.Headers.Append("X-Theory-Percentage", theoryPercentage.ToString());
        HttpContext.Response.Headers.Append("X-Practical-Percentage", practicalPercentage.ToString());

        var result = await _serviceFactory.EntranceTestService.GetEntranceTestStudentDetail(id, studentId,
            base.CurrentAccount!);

        return result.Adapt<EntranceTestStudentDetailResponse>();
    }

    [HttpDelete("{id}/students/{student-id}")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Student])]
    [EndpointDescription("Remove a student from a test")]
    public async Task<ActionResult> RemoveStudentFromTest([FromRoute(Name = "id")] Guid id,
        [FromRoute(Name = "student-id")] string studentId)
    {
        await _serviceFactory.EntranceTestService.RemoveStudentFromTest(id, studentId, base.CurrentAccount!);

        return NoContent();
    }

    [HttpPost("enrollment-requests")]
    [CustomAuthorize(Roles = [Role.Student])]
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
    [CustomAuthorize(Roles = [Role.Staff])]
    public async Task<ActionResult> AutoArrangeEntranceTests(
        [FromBody] AutoArrangeEntranceTestsRequest request)
    {
        await _serviceFactory.EntranceTestService.AutoArrangeEntranceTests(
            request.Adapt<AutoArrangeEntranceTestsModel>(), base.CurrentAccount!);
        return Created();
    }


    [HttpPut("{id}/results")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Instructor])]
    [EndpointDescription("Update results of an entrance test")]
    public async Task<ActionResult> UpdateEntranceTestResults(
        [FromRoute] Guid id,
        [FromBody] UpdateStudentsEntranceTestResultsRequest request)
    {
        await _serviceFactory.EntranceTestService.UpdateStudentsEntranceTestResults(
            request.Adapt<UpdateStudentsEntranceTestResultsModel>(), id, base.CurrentAccount!);

        return NoContent();
    }

    [HttpPut("{id}/students/{student-id}/results")]
    [EndpointDescription("Update student entrance test results")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Instructor])]
    public async Task<ActionResult> UpdateStudentEntranceResults([FromRoute] Guid id,
        [FromRoute(Name = "student-id")] string studentId,
        [FromBody] UpdateEntranceTestResultsRequest updateRequest)
    {
        await _serviceFactory.EntranceTestService.UpdateStudentEntranceResults(id, studentId,
            updateRequest.Adapt<UpdateEntranceTestResultsModel>(), base.CurrentAccount!);
        return NoContent();
    }
}