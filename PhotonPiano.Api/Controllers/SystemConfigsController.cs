using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.BusinessLogic.BusinessModel.SystemConfig;
using PhotonPiano.BusinessLogic.Interfaces;

namespace PhotonPiano.Api.Controllers;

[ApiController]
[Route("api/system-configs")]
public class SystemConfigsController : BaseController
{
    private readonly IServiceFactory _serviceFactory;

    public SystemConfigsController(IServiceFactory serviceFactory)
    {
        _serviceFactory = serviceFactory;
    }
    
    [HttpGet("attendance-deadline")]
    [EndpointDescription("Get system configs of attendance deadline")]
    public async Task<ActionResult> GetSystemConfigs()
    {
        var cacheKey = $"Deadline cho điểm danh {DateTime.Today.Year}";
        var cacheValue = await _serviceFactory.RedisCacheService.GetAsync<SystemConfigSimpleModel>(cacheKey);
        if (cacheValue != null)
        {
            return Ok(cacheValue);
        }
        var result = await _serviceFactory.SystemConfigService.GetConfig($"Deadline cho điểm danh {DateTime.Today.Year}");

        // check if the config value is empty, set it to the end of the day 
        if (string.IsNullOrEmpty(result.ConfigValue))
        {
            result.ConfigValue = DateTime.UtcNow.Date.AddDays(1).AddTicks(-1).ToString("o"); // ISO 8601 format
        }
        
        await _serviceFactory.RedisCacheService.SaveAsync(cacheKey, result, TimeSpan.FromDays(365));
        return Ok(result);
    }

}