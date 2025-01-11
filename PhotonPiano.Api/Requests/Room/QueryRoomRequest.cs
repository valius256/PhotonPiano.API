using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Requests.Query;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.Room;

public record QueryRoomRequest : QueryPagedRequest
{
    [FromQuery(Name = "keyword")]
    public string? Keyword { get; set; }
    [FromQuery(Name = "room-status")]
    public List<RoomStatus> RoomStatus { get; set; } = [];
}