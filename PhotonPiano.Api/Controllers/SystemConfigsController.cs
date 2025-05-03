using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Requests.Class;
using PhotonPiano.Api.Requests.EntranceTest;
using PhotonPiano.Api.Requests.Scheduler;
using PhotonPiano.Api.Requests.Survey;
using PhotonPiano.Api.Requests.SystemConfig;
using PhotonPiano.Api.Requests.Tution;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.Slot;
using PhotonPiano.BusinessLogic.BusinessModel.Survey;
using PhotonPiano.BusinessLogic.BusinessModel.SystemConfig;
using PhotonPiano.BusinessLogic.BusinessModel.Tuition;
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
    public async Task<ActionResult<List<SystemConfigModel>>> GetConfigs([FromQuery] QuerySystemConfigsRequest request)
    {
        return await _serviceFactory.SystemConfigService.GetConfigs(request.Names);
    }

    [HttpGet("{name}")]
    [EndpointDescription("Get system config by name")]
    public async Task<ActionResult<SystemConfigModel>> GetByName([FromRoute] string name)
    {
        return await _serviceFactory.SystemConfigService.GetConfig(name);
    }

    [HttpGet("attendance-deadline")]
    [EndpointDescription("Get system config by name")]
    public async Task<ActionResult<SystemConfigModel>> GetByAttendance()
    {
        return await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.AttendanceDeadline);
    }


    [HttpPut]
    [CustomAuthorize(Roles = [Role.Administrator])]
    [EndpointDescription("Set a system config")]
    public async Task<IActionResult> SetConfig(UpdateSystemConfigModel updateSystemConfigModel)
    {
        await _serviceFactory.SystemConfigService.SetConfigValue(updateSystemConfigModel);
        return NoContent();
    }

    [HttpGet("cancel-slot-reason")]
    [EndpointDescription("Get system configs of cancel slot reason")]
    public async Task<ActionResult> GetSystemConfigsOfCancelSlotReason()
    {
        var cacheKey = "CancelSlotReason";
        var cacheValue = await _serviceFactory.RedisCacheService.GetAsync<SystemConfigModel>(cacheKey);
        if (cacheValue != null) return Ok(cacheValue);

        var result = await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.ReasonForCancelSlot);

        await _serviceFactory.RedisCacheService.SaveAsync(cacheKey, result, TimeSpan.FromDays(365));
        return Ok(result);
    }

    [HttpGet("refund-reason")]
    [EndpointDescription("Get system configs of refund reason")]
    public async Task<ActionResult> GetSystemConfigsOfRefundReason()
    {
        var cacheKey = "RefundReason";
        var cacheValue = await _serviceFactory.RedisCacheService.GetAsync<SystemConfigModel>(cacheKey);
        if (cacheValue != null) return Ok(cacheValue);

        var result = await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.ReasonForRefund);

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

    [HttpPut("tuition")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Administrator])]
    [EndpointDescription("Update tuition system config")]
    public async Task<ActionResult> UpdateTuitionSystemConfig(
        [FromBody] UpdateTuitionSystemConfigRequest request)
    {
        await _serviceFactory.SystemConfigService.UpdateTuitionSystemConfig(
            request.Adapt<UpdateTuitionSystemConfigModel>());

        return NoContent();
    }

    [HttpPut("schedule")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Administrator])]
    [EndpointDescription("Update tuition system config")]
    public async Task<ActionResult> UpdateSchedulerSystemConfig(
        [FromBody] UpdateSchedulerSystemConfigRequest request)
    {
        await _serviceFactory.SystemConfigService.UpdateSchedulerSystemConfig(
            request.Adapt<UpdateSchedulerSystemConfigModel>());

        return NoContent();
    }

    [HttpPut("classes")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Administrator])]
    [EndpointDescription("Update tuition system config")]
    public async Task<ActionResult> UpdateClassConfig(
        [FromBody] UpdateClassSystemConfigRequest request)
    {
        await _serviceFactory.SystemConfigService.UpdateClassSystemConfig(
            request.Adapt<UpdateClassSystemConfigModel>());

        return NoContent();
    }

    [HttpPut("refund")]
    [CustomAuthorize(Roles = [Role.Staff, Role.Administrator])]
    [EndpointDescription("Update refund system config")]
    public async Task<ActionResult> UpdateRefundConfig(
        [FromBody] UpdateRefundSystemConfigRequest request)
    {
        await _serviceFactory.SystemConfigService.UpdateRefundSystemConfig(
            request.Adapt<UpdateRefundSystemConfigModel>());

        return NoContent();
    }

    [HttpGet("current-server-time")]
    [EndpointDescription("Get current server time")]
    public ActionResult<DateTime> GetCurrentServerTime()
    {
        return Ok(DateTime.UtcNow.AddHours(7));
    }
}