using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.Application;
using PhotonPiano.BusinessLogic.BusinessModel.Application;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Controllers
{
    [Route("api/applications")]
    [ApiController]
    public class ApplicationsController : BaseController
    {
        private readonly IServiceFactory _serviceFactory;

        public ApplicationsController(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        [HttpGet]
        [EndpointDescription("Get academic applications with paging")]
        [FirebaseAuthorize(Roles = [Role.Staff, Role.Student])]
        public async Task<ActionResult<List<ApplicationModel>>> GetPagedApplications([FromQuery] QueryPagedApplicationsRequest request)
        {
            var pagedResult =
                await _serviceFactory.ApplicationService.GetPagedApplications(
                    request.Adapt<QueryPagedApplicationModel>(), base.CurrentAccount!);
            
            HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);
            
            return pagedResult.Items;
        }
    }
}