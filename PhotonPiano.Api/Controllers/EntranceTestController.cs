using L2Drive.API.Requests.EntranceTestStudent;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.EntranceTest;
using PhotonPiano.Api.Requests.EntranceTestStudent;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
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
    public async Task<ActionResult<List<EntranceTestDetailModel>>> GetEntranceTest(
        [FromQuery] QueryEntranceTestRequest request)
    {
        var pagedResult =
            await _serviceFactory.EntranceTestService.GetPagedEntranceTest(
                request.Adapt<QueryEntranceTestModel>());

        HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);
        return pagedResult.Items;
    }

    [HttpGet("{id}")]
    [EndpointDescription("Get EntranceTestStudent by id")]
    public async Task<ActionResult<EntranceTestDetailModel>> GetEntranceTestById([FromRoute] Guid id)
    {
        return Ok(await _serviceFactory.EntranceTestService.GetEntranceTestDetailById(id));
    }

    [HttpPost]
    [EndpointDescription("Create EntranceTestStudent")]
    public async Task<ActionResult<EntranceTestDetailModel>> CreateEntranceTestStudent(
        [FromBody] CreateEntranceTestRequest request)
    {
        var result =
            await _serviceFactory.EntranceTestService.CreateEntranceTest(
                request.Adapt<EntranceTestModel>(), CurrentUserFirebaseId);
        return Created(nameof(CreateEntranceTestStudent), result);
    }

    [HttpDelete("{id}")]
    [EndpointDescription("Delete EntranceTestStudent")]
    public async Task<ActionResult> DeleteEntranceTestStudent([FromRoute] Guid id)
    {
        await _serviceFactory.EntranceTestService.DeleteEntranceTest(id, CurrentUserFirebaseId);
        return Ok();
    }

    [HttpPut("{id}")]
    [FirebaseAuthorize]
    [EndpointDescription("Update EntranceTestStudent")]
    public async Task<ActionResult> UpdateEntranceTest([FromRoute] Guid id, [FromBody] UpdateEntranceTestRequest request)
    {
        await _serviceFactory.EntranceTestService.UpdateEntranceTest(id, request.Adapt<UpdateEntranceTestModel>(), CurrentUserFirebaseId);

        return NoContent();
    }
}