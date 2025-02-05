using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.Scheduler;

public record SchedulerRequest
{
    [Required(ErrorMessage = "The Start time is required")]
    [FromQuery(Name = "start-time")]
    public required DateOnly StartTime { get; set; }
    [FromQuery(Name = "end-time")]
    [Required(ErrorMessage = "The End time is required")]
    public required DateOnly EndTime { get; set; }
    [FromQuery(Name = "shifts")]
    public List<Shift>? Shifts { get; set; }
    [FromQuery(Name = "slot-statuses")]
    public List<SlotStatus>? SlotStatuses { get; set; }
    [FromQuery(Name = "instructor-firebase-id")]
    public string? InstructorFirebaseId { get; set; }
    [FromQuery(Name = "student-firebase-id")]
    public string? StudentFirebaseId { get; set; }
}