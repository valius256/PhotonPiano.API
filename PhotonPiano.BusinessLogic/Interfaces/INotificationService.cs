using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface INotificationService
{
    Task SendNotificationAsync(string userFirebaseId, string title, string message);
    Task<List<Notification>> GetUserNotificationsAsync(string userId);
}