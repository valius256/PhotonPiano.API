namespace PhotonPiano.Api.Requests.Notification;

public record NotificationRequest
{
    public string Title { get; init; }
    public string Message { get; init; }
    public string UserName { get; init; }
}