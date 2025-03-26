using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.Class;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Enum;

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

        [HttpPost("auto-arrangement")]
        [FirebaseAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Arrange classes to students who are waiting for a class")]
        public async Task<ActionResult<List<ClassModel>>> ArrangeClasses(
         [FromBody] ArrangeClassModel arrangeClassModel)
        {
            return await _serviceFactory.ClassService.AutoArrangeClasses(arrangeClassModel, CurrentUserFirebaseId);
        }

        [HttpGet("{id}")]
        [EndpointDescription("Get Class detail by Id")]
        public async Task<ActionResult<ClassDetailModel>> GetClassDetail(
         [FromRoute] Guid id)
        {
            return await _serviceFactory.ClassService.GetClassDetailById(id);
        }

        [HttpGet("{id}/scoreboard")]
        [FirebaseAuthorize(Roles = [Role.Staff, Role.Instructor])]
        [EndpointDescription("Get Class scoreboard by Id")]
        public async Task<ActionResult<ClassScoreboardModel>> GetClassScoreboard(
         [FromRoute] Guid id)
        {
            return await _serviceFactory.ClassService.GetClassScoreboard(id);
        }



        [HttpPost]
        [FirebaseAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Create a class individually")]
        public async Task<ActionResult<ClassModel>> CreateClass(
        [FromBody] CreateClassRequest request)
        {
            var result =
                await _serviceFactory.ClassService.CreateClass(
                    request.Adapt<CreateClassModel>(), CurrentUserFirebaseId);
            return Created(nameof(CreateClass), result);
        }

        [HttpDelete("{id}")]
        [FirebaseAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Delete a class")]
        public async Task<ActionResult> DeleteClass([FromRoute] Guid id)
        {
            await _serviceFactory.ClassService.DeleteClass(id, CurrentUserFirebaseId);
            return NoContent();
        }

        [HttpPut]
        [FirebaseAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Update a class")]
        public async Task<ActionResult> UpdateClass([FromBody] UpdateClassRequest request)
        {
            await _serviceFactory.ClassService.UpdateClass(request.Adapt<UpdateClassModel>(),
                CurrentUserFirebaseId);

            return NoContent();
        }

        [HttpPost("student-class")]
        [FirebaseAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Add many students to a class")]
        public async Task<ActionResult<StudentClassModel>> CreateStudentClass(
        [FromBody] CreateStudentClassRequest request)
        {
            var result =
                await _serviceFactory.StudentClassService.CreateStudentClass(
                    request.Adapt<CreateStudentClassModel>(), CurrentUserFirebaseId);
            return Created(nameof(CreateStudentClass), result);
        }

        [HttpDelete("student-class")]
        [FirebaseAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Delete a studentClass")]
        public async Task<ActionResult> DeleteStudentClass([FromQuery] string studentId, [FromQuery] Guid classId, [FromQuery] bool IsExpelled = false)
        {
            await _serviceFactory.StudentClassService.DeleteStudentClass(studentId, classId, IsExpelled, CurrentUserFirebaseId);
            return NoContent();
        }

        [HttpPut("student-class")]
        [FirebaseAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Change class of a student")]
        public async Task<ActionResult> ChangeClassOfStudent([FromBody] UpdateStudentClassRequest request)
        {
            await _serviceFactory.StudentClassService.ChangeClassOfStudent(request.Adapt<ChangeClassModel>(),
                CurrentUserFirebaseId);

            return NoContent();
        }

        [HttpPatch("{classId}/publishing")]
        [FirebaseAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Publish a class")]
        public async Task<ActionResult> PublishClass([FromRoute] Guid classId)
        {
            await _serviceFactory.ClassService.PublishClass(classId, CurrentUserFirebaseId);
            return NoContent();
        }

        [HttpPatch("scheduling")]
        [FirebaseAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Schedule a class based on option")]
        public async Task<ActionResult> ScheduleClass([FromBody] ScheduleClassRequest scheduleClassRequest)
        {
            await _serviceFactory.ClassService.ScheduleClass(scheduleClassRequest.Adapt<ScheduleClassModel>(), CurrentUserFirebaseId);
            return NoContent();
        }

        [HttpDelete("{id}/schedule")]
        [FirebaseAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Delete entire schedule a class")]
        public async Task<ActionResult> DeleteClassSchedule([FromRoute] Guid id)
        {
            await _serviceFactory.ClassService.ClearClassSchedule(id, CurrentUserFirebaseId);
            return NoContent();
        }
    }
}
