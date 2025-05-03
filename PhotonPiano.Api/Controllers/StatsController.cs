using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Requests.Statistics;
using PhotonPiano.BusinessLogic.BusinessModel.Statistics;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Controllers
{
    [Route("api/stats")]
    [ApiController]
    public class StatsController : BaseController
    {
        private readonly IServiceFactory _serviceFactory;

        public StatsController(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        [HttpGet("overview-stats")]
        [CustomAuthorize(Roles = [Role.Administrator])]
        public async Task<ActionResult<List<StatModel>>> GetOverviewStats([FromQuery] QueryStatsRequest request)
        {
            return await _serviceFactory.StatisticService.GetOverviewStatsAsync(request.Month, request.Year);
        }

        [HttpGet("revenue-stats")]
        [CustomAuthorize(Roles = [Role.Administrator])]
        public async Task<ActionResult<List<StatModel>>> GetMonthlyRevenueStats([FromQuery] int year)
        {
            return await _serviceFactory.StatisticService.GetMonthlyRevenueStatsByYearAsync(year);
        }

        [HttpGet("level-stats")]
        [CustomAuthorize(Roles = [Role.Administrator])]
        public async Task<ActionResult<List<PieStatModel>>> GetLevelStats([FromQuery] string filterBy = "classes")
        {
            return await _serviceFactory.StatisticService.GetPianoLevelStatisticsAsync(filterBy);
        }
    }
}