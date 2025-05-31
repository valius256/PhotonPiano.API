using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Requests.Level;
using PhotonPiano.Api.Responses.Level;
using PhotonPiano.BusinessLogic.BusinessModel.Level;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Controllers
{
    [Route("api/levels")]
    [ApiController]
    public class LevelsController : BaseController
    {
        private readonly IServiceFactory _serviceFactory;

        public LevelsController(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        [HttpGet]
        [EndpointDescription("Get all levels")]
        public async Task<ActionResult<List<LevelModel>>> GetAllLevels()
        {
            return await _serviceFactory.LevelService.GetCachedAllLevelsAsync();
        }

        [HttpGet("{id}")]
        [EndpointDescription("Get level by id")]
        public async Task<ActionResult<LevelDetailsResponse>> GetLevel([FromRoute] Guid id)
        {
            var result = await _serviceFactory.LevelService.GetLevelDetailsAsync(id);
            return result.Adapt<LevelDetailsResponse>();
        }

        [HttpPost]
        [CustomAuthorize(Roles = [Role.Staff, Role.Administrator])]
        [EndpointDescription("Create new level")]
        public async Task<ActionResult> CreateLevel([FromBody] CreateLevelRequest request)
        {
            return Created(nameof(CreateLevel),
                await _serviceFactory.LevelService.CreateLevelAsync(request.Adapt<CreateLevelModel>(),
                    base.CurrentAccount!));
        }

        [HttpPut("{id}")]
        [CustomAuthorize(Roles = [Role.Staff, Role.Administrator])]
        [EndpointDescription("Update level")]
        public async Task<ActionResult> UpdateLevel([FromRoute] Guid id, [FromBody] UpdateLevelRequest request)
        {
            await _serviceFactory.LevelService.UpdateLevelAsync(id, request.Adapt<UpdateLevelModel>(),
                base.CurrentAccount!);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [CustomAuthorize(Roles = [Role.Staff, Role.Administrator])]
        [EndpointDescription("Delete level")]
        public async Task<ActionResult> DeleteLevel([FromRoute] Guid id, [FromQuery] Guid fallBackLevelId)
        {
            await _serviceFactory.LevelService.DeleteLevelAsync(id, fallBackLevelId);
            
            return NoContent();
        }
        
        [HttpPatch("{id}/minimum-gpa")]
        [CustomAuthorize(Roles = [Role.Staff, Role.Administrator])]
        [EndpointDescription("Update minimum GPA of a level")]
        public async Task<ActionResult> UpdateLevelMinimumGpa([FromRoute] Guid id, [FromBody] UpdateLevelMinimumGpaRequest request)
        {
            await _serviceFactory.LevelService.UpdateLevelMinimumGpaAsync(id, 
               request.Adapt<UpdateLevelMinimumGpaModel>());
            
            return NoContent();
        }

        [HttpPut("orders")]
        [CustomAuthorize(Roles = [Role.Staff, Role.Administrator])]
        [EndpointDescription("Update level order")]
        public async Task<ActionResult> UpdateLevelOrder([FromBody] UpdateLevelOrderRequest request)
        {
            await _serviceFactory.LevelService.ChangeLevelOrder(request.Adapt<UpdateLevelOrderModel>());
            return NoContent();
        }
    }
}