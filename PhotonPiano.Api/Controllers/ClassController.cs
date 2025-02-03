using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.Class;
using PhotonPiano.Api.Requests.Criteria;
using PhotonPiano.Api.Responses.Criteria;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.Criteria;
using PhotonPiano.BusinessLogic.Interfaces;

namespace PhotonPiano.Api.Controllers
{
    [ApiController]
    [Route("api/classes")]
    public class ClassController : BaseController
    {
        private readonly IServiceFactory _serviceFactory;

        public ClassController(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        [HttpGet]
        [EndpointDescription("Get Classes with Paging")]
        public async Task<ActionResult<List<ClassModel>>> GetCriteria(
         [FromQuery] QueryClassRequest request)
        {
            var pagedResult =
                await _serviceFactory.ClassService.GetPagedClasses(
                    request.Adapt<QueryClassModel>());

            HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);
            return pagedResult.Items.Adapt<List<ClassModel>>();
        }

        [HttpGet("{id}")]
        [EndpointDescription("Get Class detail by Id")]
        public async Task<ActionResult<ClassDetailModel>> GetClassDetail(
         [FromRoute] Guid id)
        {
            return await _serviceFactory.ClassService.GetClassDetailById(id);
        }

        [HttpPost("auto-arrangement")]
        [EndpointDescription("Arrange classes to students who are waiting for a class")]
        public async Task<ActionResult<List<ClassModel>>> ArrangeClasses(
         [FromBody] ArrangeClassModel arrangeClassModel)
        {
            return await _serviceFactory.ClassService.AutoArrangeClasses(arrangeClassModel);
        }
    }
}
