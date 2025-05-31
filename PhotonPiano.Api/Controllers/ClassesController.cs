using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.Class;
using PhotonPiano.Api.Requests.StudentScore;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Controllers;

[ApiController]
[Route("api/classes")]
public class ClassesController : BaseController
{
    private readonly IServiceFactory _serviceFactory;

    public ClassesController(IServiceFactory serviceFactory)
    {
        _serviceFactory = serviceFactory;
    }

    [HttpGet]
    [EndpointDescription("Get Classes with Paging")]
    public async Task<ActionResult<List<ClassModel>>> GetClasses(
        [FromQuery] QueryClassRequest request)
    {
        var pagedResult =
            await _serviceFactory.ClassService.GetPagedClasses(
                request.Adapt<QueryClassModel>(), base.CurrentAccountId);

        HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);

        return pagedResult.Items;
    }

    [HttpPost("auto-arrangement")]
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Arrange classes to students who are waiting for a class")]
    public async Task<ActionResult<List<ClassModel>>> ArrangeClasses(
        [FromBody] ArrangeClassModel arrangeClassModel)
    {
        return await _serviceFactory.ClassService.AutoArrangeClasses(arrangeClassModel, CurrentAccountId);
    }

    [HttpGet("{id}")]
    [EndpointDescription("Get Class detail by Id")]
    public async Task<ActionResult<ClassDetailModel>> GetClassDetail(
        [FromRoute] Guid id)
    {
        return await _serviceFactory.ClassService.GetClassDetailById(id);
    }

    [HttpGet("{id}/scoreboard")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Instructor])]
    [EndpointDescription("Get Class scoreboard by Id")]
    public async Task<ActionResult<ClassScoreboardModel>> GetClassScoreboard(
        [FromRoute] Guid id)
    {
        return await _serviceFactory.ClassService.GetClassScoreboard(id);
    }


    [HttpPost]
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Create a class individually")]
    public async Task<ActionResult<ClassModel>> CreateClass(
        [FromBody] CreateClassRequest request)
    {
        var result =
            await _serviceFactory.ClassService.CreateClass(
                request.Adapt<CreateClassModel>(), CurrentAccountId);
        return Created(nameof(CreateClass), result);
    }

    [HttpDelete("{id}")]
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Delete a class")]
    public async Task<ActionResult> DeleteClass([FromRoute] Guid id)
    {
        await _serviceFactory.ClassService.DeleteClass(id, CurrentAccountId);
        return NoContent();
    }

    [HttpPut]
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Update a class")]
    public async Task<ActionResult> UpdateClass([FromBody] UpdateClassRequest request)
    {
        await _serviceFactory.ClassService.UpdateClass(request.Adapt<UpdateClassModel>(),
            CurrentAccountId);

        return NoContent();
    }

    [HttpPost("student-class")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Student])]
    [EndpointDescription("Add many students to a class")]
    public async Task<ActionResult<StudentClassModel>> CreateStudentClass(
        [FromBody] CreateStudentClassRequest request)
    {
        var result =
            await _serviceFactory.StudentClassService.CreateStudentClass(
                request.Adapt<CreateStudentClassModel>(), CurrentAccount!);
        return Created(nameof(CreateStudentClass), result);
    }

    [HttpDelete("student-class")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Student])]
    [EndpointDescription("Delete a studentClass")]
    public async Task<ActionResult> DeleteStudentClass([FromQuery] string studentId, [FromQuery] Guid classId,
        [FromQuery] bool IsExpelled = false)
    {
        await _serviceFactory.StudentClassService.DeleteStudentClass(studentId, classId, IsExpelled, CurrentAccount!);
        return NoContent();
    }

    [HttpPut("student-class")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Student])]
    [EndpointDescription("Change class of a student")]
    public async Task<ActionResult> ChangeClassOfStudent([FromBody] UpdateStudentClassRequest request)
    {
        if (CurrentAccount == null) return Unauthorized();
        await _serviceFactory.StudentClassService.ChangeClassOfStudent(request.Adapt<ChangeClassModel>(),
            CurrentAccount);

        return NoContent();
    }

    [HttpPatch("{classId}/publishing")]
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Publish a class")]
    public async Task<ActionResult> PublishClass([FromRoute] Guid classId)
    {
        await _serviceFactory.ClassService.PublishClass(classId, CurrentAccountId);
        return NoContent();
    }

    [HttpPatch("scheduling")]
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Schedule a class based on option")]
    public async Task<ActionResult> ScheduleClass([FromBody] ScheduleClassRequest scheduleClassRequest)
    {
        await _serviceFactory.ClassService.ScheduleClass(scheduleClassRequest.Adapt<ScheduleClassModel>(),
            CurrentAccountId);
        return NoContent();
    }

    [HttpDelete("{id}/schedule")]
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Delete entire schedule a class")]
    public async Task<ActionResult> DeleteClassSchedule([FromRoute] Guid id)
    {
        await _serviceFactory.ClassService.ClearClassSchedule(id, CurrentAccountId);
        return NoContent();
    }

    [HttpGet("available-teachers-for-class")]
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Get available teacher to assign to a class")]
    public async Task<ActionResult<List<AccountSimpleModel>>> GetAvailableTeacher(
        [FromQuery] GetAvailableTeacherForClassRequest request)
    {
        var pagedResult =
            await _serviceFactory.ClassService.GetAvailableTeacher(
                request.Adapt<GetAvailableTeacherForClassModel>());

        HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);

        return pagedResult.Items;
    }

    [HttpPut("schedule-shifting")]
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Delay the schedule for a class")]
    public async Task<IActionResult> ShiftClassSchedule([FromBody] ShiftClassScheduleRequest request)
    {
        await _serviceFactory.ClassService.ShiftClassSchedule(request.Adapt<ShiftClassScheduleModel>(),
            CurrentAccountId);

        return NoContent();
    }

    [HttpPut("merging")]
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Mege this class to another class")]
    public async Task<IActionResult> MergeClass([FromBody] MergeClassRequest request)
    {
        await _serviceFactory.ClassService.MergeClass(request.Adapt<MergeClassModel>(),
            CurrentAccountId);

        return NoContent();
    }

    [HttpGet("{id}/merging")]
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Get mergable class for a class")]
    public async Task<ActionResult<List<ClassWithSlotsModel>>> GetMergableClass(
        [FromRoute] Guid id)
    {
        return await _serviceFactory.ClassService.GetMergableClass(id);
    }

    // Student Score Publishing (fixed)
    [HttpPost("{id}/score-publishing-status")]
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Publish Scores for a Class")]
    public async Task<IActionResult> PublishClassScores([FromRoute] Guid id)
    {
        await _serviceFactory.StudentClassScoreService.PublishScore(id, CurrentAccount!);
        return NoContent();
    }

    // Grade Template Download (fixed)
    [HttpGet("{id}/grade-template")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Instructor])]
    [EndpointDescription("Download Grade Template Excel")]
    public async Task<IActionResult> DownloadClassGradeTemplate([FromRoute] Guid id)
    {
        var templateBytes = await _serviceFactory.StudentClassService.GenerateGradeTemplate(id);
        return File(templateBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "grade_template.xlsx");
    }

    // Import Scores (fixed)
    [HttpPost("{classId}/student-scores")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Instructor])]
    [EndpointDescription("Import student scores from an Excel file")]
    public async Task<ActionResult> ImportStudentScores([FromRoute] Guid classId, IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var success = await _serviceFactory.StudentClassService.ImportScores(classId, stream, CurrentAccount!);
        return success ? NoContent() : NotFound();
    }

    // Update Student Status (fixed)
    [HttpPut("{id}/students/{student-id}/status")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Instructor])]
    [EndpointDescription("Change Student Status in Class")]
    public async Task<IActionResult> UpdateStudentClassStatus(
        [FromRoute] Guid id,
        [FromRoute(Name = "student-id")] string studentId,
        [FromBody] ChangeStudentStatusRequest request)
    {
        // Ensure the class ID in the route matches the one in the request
        if (id != request.ClassId)
        {
            return BadRequest("ClassId in route must match ClassId in request body");
        }

        var success = await _serviceFactory.StudentClassService.UpdateStudentStatusAsync(
            studentId, request.NewStatus, CurrentAccount!, id);
        return success ? NoContent() : BadRequest("Failed to change student status");
    }

    // Update Student Score
    // [HttpPut("{classId}/student/scores")]
    // [CustomAuthorize(Roles = [Role.Staff, Role.Instructor])]
    // [EndpointDescription("Update a student's score for a specific criteria")]
    // public async Task<IActionResult> UpdateStudentScore(
    //     [FromRoute] Guid classId,
    //     [FromBody] UpdateStudentScoreRequest request)
    // {
    //     var model = new UpdateStudentScoreModel
    //     {
    //         StudentClassId = request.StudentClassId,
    //         CriteriaId = request.CriteriaId,
    //         Score = request.Score
    //     };
    //
    //     var success = await _serviceFactory.StudentClassService.UpdateStudentScore(model, CurrentAccount!);
    //     return success ? NoContent() : BadRequest("Failed to update student score");
    // }

    // Batch Update Scores (fixed)
    [HttpPut("{id}/student-scores")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Instructor])]
    [EndpointDescription("Update scores for multiple students and criteria in a batch")]
    public async Task<IActionResult> UpdateBatchStudentScores(
        [FromRoute] Guid id,
        [FromBody] UpdateBatchStudentScoreRequest request)
    {
        var success = await _serviceFactory.StudentClassService.UpdateBatchStudentClassScores(
            request.Adapt<UpdateBatchStudentClassScoreModel>(), CurrentAccount!);
        return success
            ? NoContent()
            : BadRequest("Failed to update batch student scores");
    }

    // Get Class Scores (fixed)
    [HttpGet("{id}/student-scores")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Instructor])]
    [EndpointDescription("Get scores for all students in a class with criteria details")]
    public async Task<IActionResult> GetAllStudentScores([FromRoute] Guid id)
    {
        var classScores = await _serviceFactory.StudentClassScoreService.GetClassScoresWithCriteria(id);
        return Ok(classScores);
    }

    // Get Student Detailed Scores (need to fix)
    [HttpGet("{id}/students/{student-id}/detailed-scores")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Instructor, Role.Student])]
    [EndpointDescription("Get detailed scores for a specific student in a class")]
    public async Task<IActionResult> GetStudentDetailedScores(
        [FromRoute] Guid id,
        [FromRoute(Name = "student-id")] string studentId)
    {
        var detailedScores = await _serviceFactory.StudentClassScoreService.GetStudentDetailedScores(id, studentId);
        return Ok(detailedScores);
    }

    // Rollback Published Scores
    [HttpPost("{id}/scores/rollback-publish")]
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Rollback Published Scores")]
    public async Task<IActionResult> RollbackClassScorePublish([FromRoute] Guid id)
    {
        await _serviceFactory.StudentClassScoreService.RollbackPublishScores(id, CurrentAccount!);
        return NoContent();
    }
}