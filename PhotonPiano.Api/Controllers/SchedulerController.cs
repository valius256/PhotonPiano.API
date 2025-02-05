using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Requests.Scheduler;
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

    [HttpGet("slots")]
    [FirebaseAuthorize(Roles = [Role.Instructor, Role.Student, Role.Staff])]
    [EndpointDescription("Get All Slots in this Week")]
    public async Task<ActionResult> GetSchedulers([FromQuery] SchedulerRequest request)
    {
        var result =
            await _serviceFactory.SlotService.GetWeeklyScheduleAsync(request.Adapt<GetSlotModel>(),
                CurrentAccount);
        return Ok(result.Adapt<List<SlotSimpleModel>>());
    }

    [HttpGet("attendance-status/{slotId}")]
    [FirebaseAuthorize(Roles = [Role.Instructor, Role.Student, Role.Staff])]
    [EndpointDescription("Get Attendance Status for a Slot")]
    public async Task<ActionResult> GetAttendanceStatus([FromRoute] Guid slotId)
    {
        var result = await _serviceFactory.SlotService.GetAttendanceStatusAsync(slotId);
        return Ok(result);
    }

    [HttpGet("slot/{id}")]
    [FirebaseAuthorize(Roles = [Role.Instructor, Role.Student, Role.Staff])]
    [EndpointDescription("Get Slot by Id")]
    public async Task<ActionResult> GetSlotById([FromRoute] Guid id)
    {
        var result = await _serviceFactory.SlotService.GetSLotDetailById(id, CurrentAccount);
        return Ok(result.Adapt<SlotDetailModel>());
    }

    [HttpPost("update-attendance")]
    [FirebaseAuthorize(Roles = [Role.Instructor])]
    [EndpointDescription("Update Attendance by Intructor")]
    public async Task<ActionResult> UpdateAttendance([FromBody] AttendanceRequest request)
    {
        await _serviceFactory.SlotStudentService.UpdateAttentStudent(request.Adapt<UpdateAttentdanceModel>(),
            CurrentUserFirebaseId);
        return Ok("Update attendance successful");
    }
}