using PhotonPiano.DataAccess.Models.Enum;

public record StudentAttendanceModel
{
    public string StudentFirebaseId { get; set; }
    public AttendanceStatus AttendanceStatus { get; set; }
}