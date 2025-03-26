using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Requests.Query;
using PhotonPiano.DataAccess.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.Transaction;

public record QueryPagedTransactionsRequest : QueryPagedRequest, IValidatableObject
{
    [FromQuery(Name = "start-date")]
    public DateTime? StartDate { get; init; }

    [FromQuery(Name = "end-date")]
    public DateTime? EndDate { get; init; }

    [FromQuery(Name = "code")]

    public string? Code { get; init; }

    [FromQuery(Name = "id")]

    public Guid? Id { get; init; }

    [FromQuery(Name = "statuses")]

    public List<PaymentStatus> PaymentStatuses { get; init; } = [];

    [FromQuery(Name = "methods")]

    public List<PaymentMethod> PaymentMethods { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate >= EndDate)
        {
            yield return new ValidationResult("Start date must be before end date",
                [nameof(StartDate), nameof(EndDate)]);
        }
    }
}