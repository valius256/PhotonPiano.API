using Microsoft.AspNetCore.Mvc;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Enums;

namespace PhotonPiano.Api.Requests.Class
{
    public record ArrangeClassRequest
    {
        [FromQuery(Name = "allowed-shifts")] public List<Shift> AllowedShifts { get; init; } = [];

        [FromQuery(Name = "start-week")] public DateOnly StartWeek { get; init; }
    }
}
