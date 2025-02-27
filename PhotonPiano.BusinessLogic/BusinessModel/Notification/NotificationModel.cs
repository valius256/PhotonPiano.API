using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Notification;

public record NotificationModel
{
    public required Guid Id { get; init; }
    
    public required string Content { get; init; }
    
    public string Thumbnail { get; init; }
    
    public DateTime CreatedAt { get; init; }
    
    public DateTime? UpdatedAt { get; init; }
    
    public DateTime? DeletedAt { get; init; }
    
    public RecordStatus RecordStatus { get; init; }
}