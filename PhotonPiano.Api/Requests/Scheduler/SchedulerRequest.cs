using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.Scheduler;

public class SchedulerRequest
{
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
    public List<Shift>? Shifts { get; set; }
    public List<SlotStatus>? SlotStatuses { get; set; }
    public string? InstructorFirebaseId { get; set; }
    public string? StudentFirebaseId { get; set; }
}