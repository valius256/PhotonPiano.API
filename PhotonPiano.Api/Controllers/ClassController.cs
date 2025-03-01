using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.Class;
using PhotonPiano.Api.Requests.Criteria;
using PhotonPiano.Api.Requests.Scheduler;
using PhotonPiano.Api.Responses.Class;
using PhotonPiano.Api.Responses.Criteria;
using PhotonPiano.Api.Responses.Slot;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.Criteria;
using PhotonPiano.BusinessLogic.BusinessModel.Slot;
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

        [HttpGet("{id}")]
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
    }
}
