namespace PhotonPiano.Api.Responses.Scheduler;

public class StudentAttendanceResultResponse
{
    public string StudentId { get; set; }
    public decimal AttendancePercentage { get; set; }
    public int TotalSlots { get; set; }
    public int AttendedSlots { get; set; }
    public bool IsPassed { get; set; }
}