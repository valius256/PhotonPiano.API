using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.EntranceTest;
using PhotonPiano.Api.Responses.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
using PhotonPiano.BusinessLogic.Interfaces;

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
    [EndpointDescription("Get EntranceTestStudent with Paging")]
    public async Task<ActionResult<List<EntranceTestResponse>>> GetEntranceTest(
        [FromQuery] QueryEntranceTestRequest request)
    {
        var pagedResult =
            await _serviceFactory.EntranceTestService.GetPagedEntranceTest(
                request.Adapt<QueryEntranceTestModel>());

        HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);
        return pagedResult.Items.Adapt<List<EntranceTestResponse>>();
    }

    [HttpGet("{id}")]
    [EndpointDescription("Get EntranceTestStudent by id")]
    public async Task<ActionResult<EntranceTestResponse>> GetEntranceTestById([FromRoute] Guid id)
    {
        var result = await _serviceFactory.EntranceTestService.GetEntranceTestDetailById(id);
        return result.Adapt<EntranceTestResponse>();
    }

    [HttpPost]
    [FirebaseAuthorize]
    [EndpointDescription("Create EntranceTestStudent")]
    public async Task<ActionResult<EntranceTestResponse>> CreateEntranceTest(
        [FromBody] CreateEntranceTestRequest request)
    {
        var result =
            await _serviceFactory.EntranceTestService.CreateEntranceTest(
                request.Adapt<EntranceTestModel>(), CurrentUserFirebaseId);
        return Created(nameof(CreateEntranceTest), result.Adapt<EntranceTestResponse>());
    }

    [HttpDelete("{id}")]
    [FirebaseAuthorize]
    [EndpointDescription("Delete EntranceTest")]
    public async Task<ActionResult> DeleteEntranceTest([FromRoute] Guid id)
    {
        await _serviceFactory.EntranceTestService.DeleteEntranceTest(id, CurrentUserFirebaseId);
        return NoContent();
    }

    [HttpPut("{id}")]
    [FirebaseAuthorize]
    [EndpointDescription("Update EntranceTest")]
    public async Task<ActionResult> UpdateEntranceTest([FromRoute] Guid id,
        [FromBody] UpdateEntranceTestRequest request)
    {
        await _serviceFactory.EntranceTestService.UpdateEntranceTest(id, request.Adapt<UpdateEntranceTestModel>(),
            CurrentUserFirebaseId);

        return NoContent();
    }
}