using Microsoft.AspNetCore.SignalR;

namespace PhotonPiano.PubSub.Notification;

public class NotificationServiceHub : INotificationServiceHub
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationServiceHub(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotificationAsync(string userFirebaseId, string userName, string title, string message)
    {
        await _hubContext.Clients.Group(userFirebaseId).SendAsync("ReceiveNotification",
            new { UserName = userName, Title = title, Message = message });
        
    }
}