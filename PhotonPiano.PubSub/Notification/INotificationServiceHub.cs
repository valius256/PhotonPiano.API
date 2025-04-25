using System.Threading.Tasks;

namespace PhotonPiano.PubSub.Notification;

public interface INotificationServiceHub
{
    Task SendNotificationAsync(string userFirebaseId, string userName, string title, string message);
}