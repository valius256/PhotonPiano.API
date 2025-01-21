using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Requests.Scheduler;
using PhotonPiano.Api.Responses.Slot;
using PhotonPiano.BusinessLogic.BusinessModel.Slot;
using PhotonPiano.BusinessLogic.Interfaces;

namespace PhotonPiano.Api.Controllers;

[ApiController]
[Route("api/scheduler")]
public class SchedulerController : BaseController
{
    private readonly IServiceFactory _serviceFactory;

    public SchedulerController(IServiceFactory serviceFactory)
    {
        _serviceFactory = serviceFactory;
    }

    [HttpGet("slot")]
    [EndpointDescription("Get All Schedulers in this Week")]
    public async Task<ActionResult> GetSchedulers([FromQuery] SchedulerRequest request)
    {
        var result =
            await _serviceFactory.SlotService.GetSlotsAsync(request.Adapt<GetSlotModel>(), CurrentUserFirebaseId);
        return Ok(result.Adapt<List<GetSlotResponses>>());
    }
}