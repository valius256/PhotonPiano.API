namespace PhotonPiano.Api.Requests.Notification;

public record BatchUpdateNotificationsRequest
{
    public required List<Guid> NotificationIds { get; init; }
}