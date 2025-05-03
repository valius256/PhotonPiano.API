using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Requests.Scheduler;
using PhotonPiano.Api.Responses.Scheduler;
using PhotonPiano.Api.Responses.Slot;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Slot;
using PhotonPiano.BusinessLogic.BusinessModel.SlotStudent;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.PubSub.Pubsub;
using PhotonPiano.Shared.Models;

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
    [CustomAuthorize(Roles = [Role.Instructor, Role.Student, Role.Staff])]
    [EndpointDescription("Get Weekly scheduler")]
    public async Task<ActionResult> GetSchedule([FromQuery] SchedulerRequest request)
    {

        if (CurrentAccount != null)
        {
            var result =
                await _serviceFactory.SlotService.GetWeeklySchedule(request.Adapt<GetSlotModel>(),
                    CurrentAccount);
            return Ok(result.Adapt<List<SlotDetailModel>>());
        }

        return Unauthorized("The user is not authorized to access this resource.");
    }

    [HttpGet("attendance-status/{slotId}")]
    [PubSub(PubSubTopic.SCHEDULER_ATTENDANCE_GET_LIST)]
    [CustomAuthorize(Roles = [Role.Instructor, Role.Student, Role.Staff])]
    [EndpointDescription("Get Attendance Status for a Slot")]
    public async Task<ActionResult> GetAttendanceStatus([FromRoute] Guid slotId)
    {
        var result = await _serviceFactory.SlotService.GetAttendanceStatus(slotId);
        return Ok(result.Adapt<List<StudentAttendanceResponse>>());
    }


    [HttpGet("slot/{id}")]
    [CustomAuthorize(Roles = [Role.Instructor, Role.Student, Role.Staff])]
    [EndpointDescription("Get Slot by Id")]
    public async Task<ActionResult> GetSlotById([FromRoute] Guid id)
    {
        var result = await _serviceFactory.SlotService.GetSlotDetailById(id, CurrentAccount);
            return Ok(result.Adapt<SlotDetailModel>());
    }

    [HttpPost("update-attendance")]
    [CustomAuthorize(Roles = new[] { Role.Instructor })]
    [EndpointDescription("Update Attendance by Instructor")]
    public async Task<IApiResult<bool>> UpdateAttendance([FromBody] AttendanceRequest request)
    {
        var result = await _serviceFactory.SlotStudentService.UpdateAttentStudent(
            request.Adapt<UpdateAttentdanceModel>(),
            CurrentUserFirebaseId);


        return OkAsync(result, PubSubTopic.SCHEDULER_ATTENDANCE_GET_LIST);
    }

    [HttpPost]
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Create a slot individually")]
    public async Task<ActionResult<GetSlotResponses>> CreateSlot(
        [FromBody] CreateSlotRequest request)
    {
        var result =
            await _serviceFactory.SlotService.CreateSlot(
                request.Adapt<CreateSlotModel>(), CurrentUserFirebaseId);
        return Created(nameof(CreateSlot), result.Adapt<GetSlotResponses>());
    }

    [HttpDelete("{id}")]
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Delete a slot")]
    public async Task<ActionResult> DeleteSlot([FromRoute] Guid id)
    {
        await _serviceFactory.SlotService.DeleteSlot(id, CurrentUserFirebaseId);
        return NoContent();
    }

    [HttpPut]
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Update a slot")]
    public async Task<ActionResult> UpdateSlot([FromBody] UpdateSlotRequest request)
    {
        await _serviceFactory.SlotService.UpdateSlot(request.Adapt<UpdateSlotModel>(),
            CurrentUserFirebaseId);

        return NoContent();
    }

    [HttpPost("blank-slot")]
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Get All Blank Class and Shift in current Week")]
    public async Task<ActionResult> GetBlankClassAndShift([FromBody] BlankSlotAndShiftRequest request)
    {
        var result = await _serviceFactory.SlotService.GetAllBlankSlotInWeeks(request.StartDate, request.EndDate);
        return Ok(result);
    }

    [HttpPost("cancel-slot")]
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Cancel a slot")]
    public async Task<ActionResult> CancelSlot([FromBody] CancelSlotRequest request)
    {
        await _serviceFactory.SlotService.CancelSlot(request.Adapt<CancelSlotModel>(), CurrentUserFirebaseId);
        return NoContent();
    }

    [HttpPost("public-new-slot")]
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Public a new slot")]
    public async Task<ActionResult> PublicNewSlot([FromBody] PublicNewSlotRequest request)
    {
        var result = await _serviceFactory.SlotService.PublicNewSlot(request.Adapt<PublicNewSlotModel>(), CurrentUserFirebaseId);
        return Ok(result);
    }
    
    [HttpGet("available-teachers-for-slot/{slotId}")]
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Get All teacher can be assigned to this slot")]
    public async Task<ActionResult> GetAllTeacherCanBeAssignedToThisSlot([FromRoute] Guid slotId)
    {
        var result = await _serviceFactory.SlotService.GetAllTeacherCanBeAssignedToSlot(slotId, CurrentUserFirebaseId);
        return Ok(result.Adapt<List<AccountSimpleModel>>());
    }
    
    [HttpPost("assign-teacher-to-slot")]
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Assign a teacher to a slot")]
    public async Task<ActionResult> AssignTeacherToSlot([FromBody] AssignTeacherToSlotRequest request)
    {
        return Ok(await _serviceFactory.SlotService.AssignTeacherToSlot(request.SlotId, request.TeacherFirebaseId, request.Reason ,CurrentUserFirebaseId));
    }
    
    [HttpGet("class-attendance/{classId}")]
    [EndpointDescription("Get attendance results for all students in a class")]
    public async Task<ActionResult> GetAllAttendanceResultByClassId([FromRoute] Guid classId)
    {
        var result = await _serviceFactory.SlotService.GetAllAttendanceResultByClassId(classId);
        return Ok(result.Adapt<List<StudentAttendanceResultResponse>>());
    }
    
    
}