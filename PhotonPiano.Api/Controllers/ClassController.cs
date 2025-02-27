using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.Class;
using PhotonPiano.Api.Requests.Criteria;
using PhotonPiano.Api.Responses.Criteria;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.Criteria;
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
        [FirebaseAuthorize(Roles = [Role.Staff, Role.Instructor])]
        [EndpointDescription("Get Classes with Paging")]
        public async Task<ActionResult<List<ClassModel>>> GetCriteria(
         [FromQuery] QueryClassRequest request)
        {
            var pagedResult =
                await _serviceFactory.ClassService.GetPagedClasses(
                    request.Adapt<QueryClassModel>(), CurrentAccount!);

            HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);
            return pagedResult.Items.Adapt<List<ClassModel>>();
        }

        [HttpGet("{id}")]
        [FirebaseAuthorize(Roles = [Role.Staff, Role.Instructor])]
        [EndpointDescription("Get Class detail by Id")]
        public async Task<ActionResult<ClassDetailModel>> GetClassDetail(
         [FromRoute] Guid id)
        {
            return await _serviceFactory.ClassService.GetClassDetailById(id);
        }

        [HttpPost("auto-arrangement")]
        [FirebaseAuthorize(Roles = [Role.Staff])]
        [EndpointDescription("Arrange classes to students who are waiting for a class")]
        public async Task<ActionResult<List<ClassModel>>> ArrangeClasses(
         [FromBody] ArrangeClassModel arrangeClassModel)
        {
            return await _serviceFactory.ClassService.AutoArrangeClasses(arrangeClassModel, CurrentUserFirebaseId);
        }
        
                
        [HttpGet("{id}/grade-template")]
        [FirebaseAuthorize(Roles = [Role.Staff, Role.Instructor])]
        [EndpointDescription("Download Template Excel")]
        public async Task<IActionResult> DownloadGradeTemplate(Guid id)
        {
            var templateBytes = await _serviceFactory.ClassService.GenerateGradeTemplate(id);
            return File(templateBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "grade_template.xlsx");
        }
    }
}
