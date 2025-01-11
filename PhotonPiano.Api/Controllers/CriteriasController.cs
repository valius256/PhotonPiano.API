using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.Criteria;
using PhotonPiano.BusinessLogic.BusinessModel.Criteria;
using PhotonPiano.BusinessLogic.Interfaces;

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
    
    [HttpGet]
    [EndpointDescription("Get Criteria with Paging")]
    public async Task<ActionResult<List<CriteriaDetailModel>>> GetCriteria(
        [FromQuery] QueryCriteriaRequest request)
    {
        var pagedResult =
            await _serviceFactory.CriteriaService.GetPagedCriteria(
                request.Adapt<QueryCriteriaModel>());

        HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);
        return pagedResult.Items;
    }

    [HttpGet("{id}")]
    [EndpointDescription("Get Criteria by id")]
    public async Task<ActionResult<CriteriaDetailModel>> GetCriteriaById([FromRoute] Guid id)
    {
        return Ok(await _serviceFactory.CriteriaService.GetCriteriaDetailById(id));
    }

}