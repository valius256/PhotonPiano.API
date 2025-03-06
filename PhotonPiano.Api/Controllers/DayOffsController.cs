using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.Class;
using PhotonPiano.Api.Requests.DayOff;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.DayOff;
using PhotonPiano.BusinessLogic.Interfaces;

namespace PhotonPiano.Api.Controllers
{
    [ApiController]
    [Route("api/day-offs")]
    public class DayOffsController : BaseController
    {
        private readonly IServiceFactory _serviceFactory;

        public DayOffsController(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        [HttpGet]
        [EndpointDescription("Get DayOffs with Paging")]
        public async Task<ActionResult<List<DayOffModel>>> GetDayOffs(
        [FromQuery] QueryDayOffRequest request)
        {
            var pagedResult =
                await _serviceFactory.DayOffService.GetPagedDayOffs(
                    request.Adapt<QueryDayOffModel>());

            HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);
            return pagedResult.Items.Adapt<List<DayOffModel>>();
        }

    }
}
