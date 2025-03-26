using Microsoft.AspNetCore.Mvc;
using PhotonPiano.BusinessLogic.BusinessModel.Level;
using PhotonPiano.BusinessLogic.Interfaces;

namespace PhotonPiano.Api.Controllers
{
    [Route("api/levels")]
    [ApiController]
    public class LevelsController : ControllerBase
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
    }
}
