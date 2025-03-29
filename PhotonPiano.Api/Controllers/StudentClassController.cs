using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
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
    [FirebaseAuthorize(Roles = [Role.Staff])]
    [EndpointDescription("Publish Score")]
    public async Task<IActionResult> PublishScore( [FromRoute] Guid classId)
    {
        await _serviceFactory.StudentClassScoreService.PublishScore(classId, CurrentAccount!);
        return NoContent();
    }
    
    [HttpGet("{id}/grade-template")]
    [FirebaseAuthorize(Roles = [Role.Staff, Role.Instructor])]
    [EndpointDescription("Download Template Excel")]
    public async Task<IActionResult> DownloadGradeTemplate(Guid id)
    {
        var templateBytes = await _serviceFactory.StudentClassService.GenerateGradeTemplate(id);
        return File(templateBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "grade_template.xlsx");
    }

    [HttpPost("{classId}/import-scores")]
    [FirebaseAuthorize(Roles = [Role.Staff, Role.Instructor])]
    [EndpointDescription("Import student scores from an Excel file")]
    public async Task<ActionResult> ImportScoresFromExcel([FromRoute] Guid classId, IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var success = await _serviceFactory.StudentClassService.ImportScores(classId, stream, CurrentAccount!);
        return success ? NoContent() : NotFound();
    } 
}