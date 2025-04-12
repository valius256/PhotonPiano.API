using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Requests.EntranceTest;
using PhotonPiano.Api.Requests.Survey;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.Survey;
using PhotonPiano.BusinessLogic.BusinessModel.SystemConfig;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Utils;

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

    [HttpGet]
    [CustomAuthorize(Roles = [Role.Staff, Role.Administrator])]
    [EndpointDescription("Get all system configs")]
    public async Task<ActionResult<List<SystemConfigModel>>> GetAll()
    {
        return await _serviceFactory.SystemConfigService.GetAllConfigs();
    }

    [HttpGet("{name}")]
    [EndpointDescription("Get system config by name")]
    public async Task<ActionResult<SystemConfigModel>> GetByName([FromRoute] string name)
    {
        return await _serviceFactory.SystemConfigService.GetConfig(name);
    }

    [HttpPut]
    [CustomAuthorize(Roles = [Role.Administrator])]
    [EndpointDescription("Set a system config")]
    public async Task<IActionResult> SetConfig(UpdateSystemConfigModel updateSystemConfigModel)
    {
        await _serviceFactory.SystemConfigService.SetConfigValue(updateSystemConfigModel);
        return NoContent();
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

        var result =
            await _serviceFactory.SystemConfigService.GetConfig($"Deadline cho điểm danh {DateTime.Today.Year}");

        // check if the config value is empty, set it to the end of the day 
        if (string.IsNullOrEmpty(result.ConfigValue))
        {
            result.ConfigValue = DateTime.UtcNow.Date.AddDays(1).AddTicks(-1).ToString("o"); // ISO 8601 format
        }

        await _serviceFactory.RedisCacheService.SaveAsync(cacheKey, result, TimeSpan.FromDays(365));
        return Ok(result);
    }

    [HttpGet("cancel-slot-reason")]
    [EndpointDescription("Get system configs of cancel slot reason")]
    public async Task<ActionResult> GetSystemConfigsOfCancelSlotReason()
    {
        var cacheKey = "CancelSlotReason";
        var cacheValue = await _serviceFactory.RedisCacheService.GetAsync<SystemConfigModel>(cacheKey);
        if (cacheValue != null)
        {
            return Ok(cacheValue);
        }

        var result = await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.ReasonForCancelSlot);

        await _serviceFactory.RedisCacheService.SaveAsync(cacheKey, result, TimeSpan.FromDays(365));
        return Ok(result);
    }

    [HttpPut("survey")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Administrator])]
    [EndpointDescription("Update survey system config")]
    public async Task<ActionResult> UpdateSurveySystemConfig([FromBody] UpdateSurveySystemConfigRequest request)
    {
        await _serviceFactory.SystemConfigService.UpdateSurveySystemConfig(
            request.Adapt<UpdateSurveySystemConfigModel>());

        return NoContent();
    }

    [HttpPut("entrance-test")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Administrator])]
    [EndpointDescription("Update entrance test system config")]
    public async Task<ActionResult> UpdateEntranceTestSystemConfig(
        [FromBody] UpdateEntranceTestSystemConfigRequest request)
    {
        await _serviceFactory.SystemConfigService.UpdateEntranceTestSystemConfig(
            request.Adapt<UpdateEntranceTestSystemConfigModel>());
        
        return NoContent();
    }
}