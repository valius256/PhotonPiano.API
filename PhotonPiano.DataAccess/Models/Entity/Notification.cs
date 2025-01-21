namespace PhotonPiano.DataAccess.Models.Entity;

public class Notification : BaseEntityWithId
{
    public required string Content { get; set; }
    public required List<string> ReceiverFirebaseIds { get; set; }
    public bool IsViewed { get; set; } = false;
    


    // Reference 
    public virtual ICollection<AccountNotification> AccountNotifications { get; set; } =
        new List<AccountNotification>();
    
}