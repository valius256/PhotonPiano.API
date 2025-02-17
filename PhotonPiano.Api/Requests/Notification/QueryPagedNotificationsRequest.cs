using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Requests.Query;

namespace PhotonPiano.Api.Requests.Notification;

public record QueryPagedNotificationsRequest : QueryPagedRequest
{
    [FromQuery(Name = "viewed")]
    public bool? IsViewed { get; init; }
}