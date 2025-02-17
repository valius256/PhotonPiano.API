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
        public async Task<ActionResult<List<ApplicationModel>>> GetPagedApplications(
            [FromQuery] QueryPagedApplicationsRequest request)
        {
            var pagedResult =
                await _serviceFactory.ApplicationService.GetPagedApplications(
                    request.Adapt<QueryPagedApplicationModel>(), base.CurrentAccount!);

            HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);

            return pagedResult.Items;
        }

        [HttpPost]
        [FirebaseAuthorize(Roles = [Role.Student])]
        [EndpointDescription("Send an application")]
        public async Task<ActionResult> SendApplication([FromForm] SendApplicationRequest request)
        {
            return Created(nameof(SendApplication),
                await _serviceFactory.ApplicationService.SendAnApplication(request.Adapt<SendApplicationModel>(),
                    base.CurrentAccount!));
        }

        [HttpPut("{id}/status")]
        [FirebaseAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Update an application status")]
        public async Task<ActionResult> UpdateApplicationStatus([FromRoute] Guid id,
            [FromBody] UpdateApplicationRequest request)
        {
            await _serviceFactory.ApplicationService.UpdateApplicationStatus(id,
                request.Adapt<UpdateApplicationModel>(), base.CurrentAccount!);
            
            return NoContent();
        }
    }
}