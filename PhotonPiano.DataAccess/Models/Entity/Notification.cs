namespace PhotonPiano.DataAccess.Models.Entity;

public class Notification : BaseEntityWithId
{
    public required string Content { get; set; }
    public required string ReceiverFirebaseId { get; set; }
    public bool IsViewed { get; set; }
    public required string SenderFirebaseId { get; set; }


    // Reference 
    public virtual Account Receiver { get; set; } = default!;
    public virtual Account Sender { get; set; } = default!;
}