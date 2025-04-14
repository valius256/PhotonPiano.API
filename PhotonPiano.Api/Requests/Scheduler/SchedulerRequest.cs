using Microsoft.AspNetCore.Mvc;
using PhotonPiano.DataAccess.Models.Enum;
using System.ComponentModel.DataAnnotations;
using PhotonPiano.Shared.Enums;

namespace PhotonPiano.Api.Requests.Scheduler;

public record SchedulerRequest
{
    [Required(ErrorMessage = "The Start time is required")]
    [FromQuery(Name = "start-time")]
    public DateOnly? StartTime { get; set; }

    [FromQuery(Name = "end-time")]
    [Required(ErrorMessage = "The End time is required")]
    public DateOnly? EndTime { get; set; }

    [FromQuery(Name = "shifts")] public List<Shift>? Shifts { get; set; } = [];

    [FromQuery(Name = "slot-statuses")] public List<SlotStatus>? SlotStatuses { get; set; } = [];

    [FromQuery(Name = "instructor-firebase-ids")]
    public List<string>? InstructorFirebaseIds { get; set; } = [];

    [FromQuery(Name = "student-firebase-id")]
    public string? StudentFirebaseId { get; set; }

    [FromQuery(Name = "class-ids")] public List<Guid>? ClassIds { get; init; } = [];
}