using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.Criteria;
using PhotonPiano.Api.Responses.Criteria;
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
    [FirebaseAuthorize(Roles = [Role.Staff, Role.Student, Role.Instructor])]
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
}