using L2Drive.API.Requests.EntranceTestStudent;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.EntranceTestStudent;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;
using PhotonPiano.BusinessLogic.Interfaces;

namespace PhotonPiano.Api.Controllers;

[ApiController]
[Route("api/entranceTest")]
public class EntranceTestController : BaseController
{
    private readonly IServiceFactory _serviceFactory;

    public EntranceTestController(IServiceFactory serviceFactory)
    {
        _serviceFactory = serviceFactory;
    }

    [HttpGet]
    [EndpointDescription("Get EntranceTestStudent with Paging")]
    public async Task<ActionResult<List<EntranceTestStudentWithEntranceTestAndStudentAccountModel>>> GetQuizzes(
        [FromQuery] QueryPagedEntranceTestStudentRequest request)
    {
        var pagedResult =
            await _serviceFactory.EntranceTestStudentService.GetPagedEntranceTest(
                request.Adapt<QueryEntranceTestStudentModel>());

        HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);
        return pagedResult.Items;
    }

    [HttpGet("{id}")]
    [EndpointDescription("Get EntranceTestStudent by id")]
    public async Task<ActionResult<EntranceTestStudentWithEntranceTestAndStudentAccountModel>> GetEntranceTestById([FromRoute] Guid id)
    {
        return Ok(await _serviceFactory.EntranceTestStudentService.GetEntranceTestStudentDetailById(id));
    }

    [HttpPost]
    [EndpointDescription("Create EntranceTestStudent")]
    public async Task<ActionResult<EntranceTestStudentWithEntranceTestAndStudentAccountModel>> CreateEntranceTestStudent(
        [FromBody] CreateEntranceTestStudentRequest request)
    {
        var result =
            await _serviceFactory.EntranceTestStudentService.CreateEntranceTestStudent(
                request.Adapt<EntranceTestStudentModel>(), CurrentUserFirebaseId);
        return Created(nameof(CreateEntranceTestStudent), result);
    }

    [HttpDelete("{id}")]
    [EndpointDescription("Delete EntranceTestStudent")]
    public async Task<ActionResult> DeleteEntranceTestStudent([FromRoute] Guid id)
    {
        await _serviceFactory.EntranceTestStudentService.DeleteEntranceTestStudent(id, CurrentUserFirebaseId);
        return Ok();
    }

    [HttpPut("{id}")]
    [FirebaseAuthorize]
    [EndpointDescription("Update EntranceTestStudent")]
    public async Task<ActionResult> UpdateQuiz([FromRoute] Guid id, [FromBody] UpdateEntranceTestStudentRequest request)
    {
        await _serviceFactory.EntranceTestStudentService.UpdateEntranceTestStudent(id, request.Adapt<UpdateEntranceTestStudentModel>(), CurrentUserFirebaseId);

        return NoContent();
    }
}