using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Requests.Query;

namespace PhotonPiano.Api.Requests.EntranceTestStudent;

public record QueryPagedEntranceTestStudentRequest : QueryPagedRequest
{
    [FromQuery(Name = "studentfirebase-ids")]
    public List<string>? StudentsFirebaseIds { get; init; } = [];

    [FromQuery(Name = "bandscore")] public decimal BandScore { get; init; }

    [FromQuery(Name = "entrancetest-ids")] public List<Guid>? EntranceTestIds { get; init; } = [];
}