namespace PhotonPiano.DataAccess.Models.Entity;

public class Notification : BaseEntityWithId
{
    public required string Content { get; set; }
    public string Thumbnail { get; set; }
    public bool IsViewed { get; set; }

    // Reference 
    public virtual ICollection<AccountNotification> AccountNotifications { get; set; } =
        new List<AccountNotification>();
}