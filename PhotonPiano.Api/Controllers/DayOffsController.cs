using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.DayOff;
using PhotonPiano.BusinessLogic.BusinessModel.DayOff;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Enum;

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

        [HttpPost]
        [CustomAuthorize(Roles = [Role.Administrator])]
        [EndpointDescription("Create DayOff")]
        public async Task<ActionResult<DayOffModel>> CreateDayOff([FromBody] CreateDayOffRequest request)
        {
            return Created(nameof(CreateDayOff),
                await _serviceFactory.DayOffService.CreateDayOff(request.Adapt<CreateDayOffModel>(),
                    base.CurrentAccount!));
        }

        [HttpPut("{id}")]
        [CustomAuthorize(Roles = [Role.Administrator])]
        [EndpointDescription("Update DayOff")]
        public async Task<ActionResult> UpdateDayOff(
            [FromRoute] Guid id,
            [FromBody] UpdateDayOffRequest request)
        {
            await _serviceFactory.DayOffService.UpdateDayOff(id, request.Adapt<UpdateDayOffModel>(),
                base.CurrentAccount!);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [CustomAuthorize(Roles = [Role.Administrator])]
        [EndpointDescription("Delete DayOff")]
        public async Task<ActionResult> DeleteDayOff([FromRoute] Guid id)
        {
            await _serviceFactory.DayOffService.DeleteDayOff(id, base.CurrentAccount!);
            return NoContent();
        }
    }
}