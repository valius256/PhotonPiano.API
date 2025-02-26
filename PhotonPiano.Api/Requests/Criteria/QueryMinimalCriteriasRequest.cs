using Microsoft.AspNetCore.Mvc;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.Criteria;

public record QueryMinimalCriteriasRequest
{
    [FromQuery(Name = "for")] public CriteriaFor CriteriaFor { get; init; } = CriteriaFor.EntranceTest;
}