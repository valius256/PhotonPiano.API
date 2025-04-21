using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.Scheduler;

public record UpdateSchedulerSystemConfigRequest
{
    [Range(1, 24, ErrorMessage = "Number of Deadline for attendance  must be between 1 and 24.")]
    public int? DeadlineAttendance { get; init; }
    
    public List<string> ReasonCancelSlot { get; init; } 
}   