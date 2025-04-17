using Microsoft.AspNetCore.Mvc;

namespace PhotonPiano.Api.Requests.SystemConfig;

public record QuerySystemConfigsRequest
{
    [FromQuery(Name = "names")]
    public List<string> Names { get; init; } = [];
}