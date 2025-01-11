using L2Drive.API.Requests.Query;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.Room;

public record QueryRoomRequest : QueryPagedRequest
{
    [FromQuery(Name = "keyword")]
    public string? Keyword { get; set; }
    [FromQuery(Name = "room-status")]
    public List<RoomStatus> RoomStatus { get; set; } = [];
}