using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.Api.Requests.Notification;
using PhotonPiano.PubSub.Notification;

namespace PhotonPiano.Api.Controllers;

[ApiController]
[Route("api/notification")]
public class NotificationController : BaseController
{
    private readonly INotificationServiceHub _notificationServiceHub;

    public NotificationController(INotificationServiceHub notificationServiceHub)
    {
        _notificationServiceHub = notificationServiceHub;
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
}