using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Requests.Scheduler;
using PhotonPiano.Api.Responses.Slot;
using PhotonPiano.BusinessLogic.BusinessModel.Slot;
using PhotonPiano.BusinessLogic.BusinessModel.SlotStudent;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Enum;

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

    [HttpGet("slot/{id}")]
    [EndpointDescription("Get Slot by Id")]
    public async Task<ActionResult> GetSlotById([FromRoute] Guid id)
    {
        var result = await _serviceFactory.SlotService.GetSLotDetailById(id);
        return Ok(result.Adapt<SlotDetailModel>());
    }

    [HttpPost("update-attendance")]
    [FirebaseAuthorize(Roles = [Role.Instructor])]
    [EndpointDescription("Update Attendance by Intructor")]
    public async Task<ActionResult> UpdateAttendance([FromBody] AttendanceRequest request)
    {
        await _serviceFactory.SLotStudentService.UpdateAttentStudent(request.Adapt<UpdateAttentdanceModel>(),
            CurrentUserFirebaseId);
        return Ok("Update attendance successful");
    }
}