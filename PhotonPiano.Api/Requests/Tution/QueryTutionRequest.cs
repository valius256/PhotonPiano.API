using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Requests.Query;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.Tution;

public record QueryTutionRequest : QueryPagedRequest
{
    [FromQuery(Name = "student-class-ids")]
    public List<Guid>? StudentClassId { get; init; } = [];

    [FromQuery(Name = "start-date")] public DateTime? StartDate { get; init; }
    [FromQuery(Name = "end-date")] public DateTime? EndDate { get; init; }
    [FromQuery(Name = "payment-statuses")] public List<PaymentStatus>? PaymentStatus { get; init; }
}