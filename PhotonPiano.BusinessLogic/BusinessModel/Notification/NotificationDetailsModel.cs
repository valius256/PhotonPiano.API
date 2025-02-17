namespace PhotonPiano.BusinessLogic.BusinessModel.Notification;

public record NotificationDetailsModel : NotificationModel
{
    public ICollection<AccountNotificationModel> AccountNotifications { get; init; } = [];
}