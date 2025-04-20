using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace PhotonPiano.Api.Requests.Scheduler;

public record BlankSlotAndShiftRequest
{
    [Required(ErrorMessage = "The Start date is required")]
    [FromQuery(Name = "start-date")]
    public DateOnly? StartDate { get; init; }
    [Required(ErrorMessage = "The End date is required")]
    [FromQuery(Name = "end-date")]
    public DateOnly? EndDate { get; init; }
}