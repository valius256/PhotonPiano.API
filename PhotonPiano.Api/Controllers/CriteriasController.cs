using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.Class;
using PhotonPiano.Api.Requests.Criteria;
using PhotonPiano.Api.Responses.Criteria;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.Criteria;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Controllers;

[ApiController]
[Route("api/criterias")]
public class CriteriasController : BaseController
{
    private readonly IServiceFactory _serviceFactory;

    public CriteriasController(IServiceFactory serviceFactory)
    {
        _serviceFactory = serviceFactory;
    }

    [HttpGet("all-minimal")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Student, Role.Instructor, Role.Administrator])]
    [EndpointDescription("Get all minimal criterias")]
    public async Task<ActionResult<List<MinimalCriteriaModel>>> GetMinimalCriterias(
        [FromQuery] QueryMinimalCriteriasRequest request)
    {
        return await _serviceFactory.CriteriaService.GetMinimalCriterias(request.Adapt<QueryMinimalCriteriasModel>());
    }

    [HttpGet]
    [EndpointDescription("Get Criteria with Paging")]
    public async Task<ActionResult<List<CriteriaResponse>>> GetCriteria(
        [FromQuery] QueryCriteriaRequest request)
    {
        var pagedResult =
            await _serviceFactory.CriteriaService.GetPagedCriteria(
                request.Adapt<QueryCriteriaModel>());

        HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);
        return pagedResult.Items.Adapt<List<CriteriaResponse>>();
    }

    [HttpGet("{id}")]
    [EndpointDescription("Get Criteria by id")]
    public async Task<ActionResult<CriteriaDetailModel>> GetCriteriaById([FromRoute] Guid id)
    {
        return Ok(await _serviceFactory.CriteriaService.GetCriteriaDetailById(id));
    }

    [HttpPost]
    [CustomAuthorize(Roles = [Role.Administrator])]
    [EndpointDescription("Create a criteria")]
    public async Task<ActionResult<CriteriaModel>> CreateCriteria(
        [FromBody] CreateCriteriaRequest request)
    {
        var result =
            await _serviceFactory.CriteriaService.CreateCriteria(
                request.Adapt<CreateCriteriaModel>(), CurrentUserFirebaseId);
        return Created(nameof(CreateCriteria), result);
    }

    [HttpDelete("{id}")]
    [CustomAuthorize(Roles = [Role.Administrator])]
    [EndpointDescription("Delete a criteria")]
    public async Task<ActionResult> DeleteCriteria([FromRoute] Guid id)
    {
        await _serviceFactory.CriteriaService.DeleteCriteria(id, CurrentUserFirebaseId);
        return NoContent();
    }

    [HttpPut]
    [CustomAuthorize(Roles = [Role.Administrator])]
    [EndpointDescription("Update a criteria")]
    public async Task<ActionResult> UpdateCriteria([FromBody] BulkUpdateCriteriaRequest request)
    {
        await _serviceFactory.CriteriaService.UpdateCriteria(request.Adapt<BulkUpdateCriteriaModel>(),
            CurrentUserFirebaseId);

        return NoContent();
    }
}