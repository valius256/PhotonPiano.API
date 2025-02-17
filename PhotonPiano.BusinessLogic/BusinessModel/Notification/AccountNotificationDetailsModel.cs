namespace PhotonPiano.BusinessLogic.BusinessModel.Notification;

public record AccountNotificationDetailsModel
{
    public required string AccountFirebaseId { get; init; }
    
    public required Guid NotificationId { get; init; }
    
    public bool IsViewed { get; init; } = false;
    
    public NotificationModel Notification { get; init; } = default!;
}