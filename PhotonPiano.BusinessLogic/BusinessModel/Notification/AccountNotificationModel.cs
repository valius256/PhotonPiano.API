namespace PhotonPiano.BusinessLogic.BusinessModel.Notification;

public record AccountNotificationModel
{
    public required string AccountFirebaseId { get; init; }
    
    public required Guid NotificationId { get; init; }
    
    public bool IsViewed { get; init; } = false;
}