using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Extensions;
using PhotonPiano.Api.Requests.Notification;
using PhotonPiano.BusinessLogic.BusinessModel.Notification;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.PubSub.Notification;

namespace PhotonPiano.Api.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationController : BaseController
{
    private readonly INotificationServiceHub _notificationServiceHub;

    private readonly IServiceFactory _serviceFactory;

    public NotificationController(INotificationServiceHub notificationServiceHub, IServiceFactory serviceFactory)
    {
        _notificationServiceHub = notificationServiceHub;
        _serviceFactory = serviceFactory;
    }

    [HttpGet]
    [EndpointDescription("Get notifications with paging")]
    [FirebaseAuthorize]
    public async Task<ActionResult<List<NotificationDetailsModel>>> GetPagedNotifications([FromQuery] QueryPagedNotificationsRequest request)
    {
        var pagedResult =
            await _serviceFactory.NotificationService.GetPagedNotifications(
                request.Adapt<QueryPagedNotificationsModel>(), base.CurrentAccount!);
        
        HttpContext.Response.Headers.AppendPagedResultMetaData(pagedResult);
        
        return pagedResult.Items;
    }

    [EndpointDescription("For testing purpose")]
    [FirebaseAuthorize]
    [HttpPost("send")]
    public async Task<IActionResult> SendNotification(NotificationRequest request)
    {
        await _notificationServiceHub.SendNotificationAsync(CurrentUserFirebaseId, request.UserName, request.Title,
            request.Message);
        return Ok();
    }

    [HttpPut("{id}/view-status")]
    [EndpointDescription("Toggle notification view status")]
    [FirebaseAuthorize]
    public async Task<ActionResult> ToggleNotificationViewStatus([FromRoute] Guid id)
    {
        await _serviceFactory.NotificationService.ToggleNotificationViewStatus(id, base.CurrentAccount!.AccountFirebaseId);
        return NoContent();
    }
}