using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Requests.Query;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.Tution;

public record QueryTutionRequest : QueryPagedRequest
{
    [FromQuery(Name = "student-class-ids")]
    public List<Guid>? StudentClassIds { get; init; } = [];

    [FromQuery(Name = "start-date")] public DateOnly? StartDate { get; init; }
    [FromQuery(Name = "end-date")] public DateOnly? EndDate { get; init; }
    [FromQuery(Name = "payment-statuses")] public List<PaymentStatus>? PaymentStatuses { get; init; }
}