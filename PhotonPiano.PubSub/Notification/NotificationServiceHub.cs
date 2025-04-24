using System.Threading.Tasks;
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
    
    public async Task SendNotificationToUserAsync(string userId, string eventName, object message)
    {
        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
        }

        if (string.IsNullOrEmpty(eventName))
        {
            throw new ArgumentException("Event name cannot be null or empty", nameof(eventName));
        }

        try
        {
            var payload = new
            {
                topic = eventName,
                content = message
            };

            await _hubContext.Clients.Group(userId).SendAsync("ReceiveNotification", payload);
            Console.WriteLine($"[NotificationServiceHub] Sent {eventName} notification to user {userId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[NotificationServiceHub] Error sending notification to user {userId}: {ex.Message}");
            throw;
        }
    }
}