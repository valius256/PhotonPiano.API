using L2Drive.API.Requests.Query;
using Microsoft.AspNetCore.Mvc;

namespace L2Drive.API.Requests.EntranceTestStudent;

public class QueryPagedEntranceTestStudentRequest : QueryPagedRequest
{
    [FromQuery(Name = "studentfirebase-ids")]
    public List<string>? StudentsFirebaseIds { get; init; } = [];
    [FromQuery(Name = "bandscore")]
    public decimal BandScore { get; init; }
    [FromQuery(Name = "entrancetest-ids")]
    public List<Guid>? EntranceTestIds { get; init; } = [];

}