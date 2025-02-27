namespace PhotonPiano.DataAccess.Models.Entity;

public class AccountNotification : BaseEntity
{
    public required string AccountFirebaseId { get; set; }
    public required Guid NotificationId { get; set; }
    public bool IsViewed { get; set; } = false;

    // reference 
    public virtual Account Account { get; set; } = default!;
    public virtual Notification Notification { get; set; } = default!;
}