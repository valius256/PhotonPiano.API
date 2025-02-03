using System.ComponentModel.DataAnnotations;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.Scheduler;

public record SchedulerRequest
{
    [Required(ErrorMessage = "The Start time is required")]
    public required DateOnly StartTime { get; set; }

    [Required(ErrorMessage = "The End time is required")]
    public required DateOnly EndTime { get; set; }

    public List<Shift>? Shifts { get; set; }
    public List<SlotStatus>? SlotStatuses { get; set; }
    public string? InstructorFirebaseId { get; set; }
    public string? StudentFirebaseId { get; set; }
}