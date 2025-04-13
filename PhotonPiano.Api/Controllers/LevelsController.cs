using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
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
        [CustomAuthorize(Roles = [Role.Staff, Role.Administrator])]
        [EndpointDescription("Get level by id")]
        public async Task<ActionResult<LevelDetailsModel>> GetLevel([FromRoute] Guid id)
        {
            return await _serviceFactory.LevelService.GetLevelDetailsAsync(id);
        }
    }
}
