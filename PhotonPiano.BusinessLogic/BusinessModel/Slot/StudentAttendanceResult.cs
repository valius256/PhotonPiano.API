namespace PhotonPiano.BusinessLogic.BusinessModel.Slot;

public record StudentAttendanceResult
{
    public string StudentId { get; set; } = default!;
    public decimal AttendancePercentage { get; set; }
    public bool IsPassed { get; set; }
    public int TotalSlots { get; set; }
    public int AttendedSlots { get; set; }
}