using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Requests.Class;
using PhotonPiano.Api.Requests.StudentScore;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Controllers;

[Route("api/student-class")]
[ApiController]
public class StudentClassController : BaseController
{
    private readonly IServiceFactory _serviceFactory;

    public StudentClassController(IServiceFactory serviceFactory)
    {
        _serviceFactory = serviceFactory;
    }

    [HttpPost("{classId}/publish-score")]
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Publish Score")]
    public async Task<IActionResult> PublishScore( [FromRoute] Guid classId)
    {
        await _serviceFactory.StudentClassScoreService.PublishScore(classId, CurrentAccount!);
        return NoContent();
    }
    
    [HttpGet("{id}/grade-template")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Instructor])]
    [EndpointDescription("Download Template Excel")]
    public async Task<IActionResult> DownloadGradeTemplate(Guid id)
    {
        var templateBytes = await _serviceFactory.StudentClassService.GenerateGradeTemplate(id);
        return File(templateBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "grade_template.xlsx");
    }

    [HttpPost("{classId}/import-scores")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Instructor])]
    [EndpointDescription("Import student scores from an Excel file")]
    public async Task<ActionResult> ImportScoresFromExcel([FromRoute] Guid classId, IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var success = await _serviceFactory.StudentClassService.ImportScores(classId, stream, CurrentAccount!);
        return success ? NoContent() : NotFound();
    }

    [HttpPut("{studentFirebaseId}/status")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Instructor])]
    [EndpointDescription("Change Student Status")]
    public async Task<IActionResult> UpdateStudentStatus([FromRoute] string studentFirebaseId, [FromBody] ChangeStudentStatusRequest request)
    {
        var success = await _serviceFactory.StudentClassService.UpdateStudentStatusAsync(
            studentFirebaseId, request.NewStatus, CurrentAccount!, request.ClassId);
        return success ? NoContent() : BadRequest("Failed to change student status");
    }
    
    [HttpPut]
    [CustomAuthorize(Roles = [Role.Staff, Role.Instructor])]
    [EndpointDescription("Update a student's score for a specific criteria")]
    public async Task<IActionResult> UpdateStudentScore([FromBody] UpdateStudentScoreRequest request)
    {
        var model = new UpdateStudentScoreModel
        {
            StudentClassId = request.StudentClassId,
            CriteriaId = request.CriteriaId,
            Score = request.Score
        };

        var success = await _serviceFactory.StudentClassService.UpdateStudentScore(model, CurrentAccount!);
        return success ? NoContent() : BadRequest("Failed to update student score");
    }
        
    [HttpPut("batch-update-scores")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Instructor])]
    [EndpointDescription("Update scores for multiple students and criteria in a batch")]
    public async Task<IActionResult> UpdateBatchScores([FromBody] UpdateBatchStudentScoreRequest request)
    {
        var success = await _serviceFactory.StudentClassService.UpdateBatchStudentClassScores(request.Adapt<UpdateBatchStudentClassScoreModel>(), CurrentAccount!);
        return success 
            ? NoContent() 
            : BadRequest("Failed to update batch student scores");
    }
    
    [HttpGet("{classId}/scores")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Instructor])]
    [EndpointDescription("Get scores for all students in a class with criteria details")]
    public async Task<IActionResult> GetClassScores([FromRoute] Guid classId)
    {
        var classScores = await _serviceFactory.StudentClassScoreService.GetClassScoresWithCriteria(classId);
        return Ok(classScores);
    }

    [HttpGet("{studentClassId}/detailed-scores")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Instructor, Role.Student])]
    [EndpointDescription("Get detailed scores for a specific student in a class")]
    public async Task<IActionResult> GetStudentDetailedScores([FromRoute] Guid studentClassId)
    {
        var detailedScores = await _serviceFactory.StudentClassScoreService.GetStudentDetailedScores(studentClassId);
        return Ok(detailedScores);
    }
    
    [HttpPost("{classId}/rollback-score-publish")]
    [CustomAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Rollback Published Scores")]
    public async Task<IActionResult> RollbackPublishScore([FromRoute] Guid classId)
    {
        await _serviceFactory.StudentClassScoreService.RollbackPublishScores(classId, CurrentAccount!);
        return NoContent();
    }
}