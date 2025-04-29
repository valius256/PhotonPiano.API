using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.Scheduler;

public record UpdateSchedulerSystemConfigRequest
{
    [Range(1, 24, ErrorMessage = "Number of Deadline for attendance must be between 1 and 24.")]
    public int? DeadlineAttendance { get; init; }

    [MinLength(1, ErrorMessage = "At least one reason for canceling slot is required")]
    [Required(ErrorMessage = "Reasons for canceling slot are required")]
    public List<string> ReasonCancelSlot { get; init; } = new();

    [Range(0, 1, ErrorMessage = "Maximum absence rate must be between 0 and 1 (e.g., 0.2 for 20%)")]
    [RegularExpression(@"^\d*\.?\d{1,2}$", ErrorMessage = "Maximum absence rate must have maximum 2 decimal places")]
    public decimal? MaxAbsenceRate { get; init; }
}