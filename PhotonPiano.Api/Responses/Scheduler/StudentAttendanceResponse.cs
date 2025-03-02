using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Responses.Scheduler
{
    public record StudentAttendanceResponse
    {
        public string StudentFirebaseId { get; set; }
        public AttendanceStatus AttendanceStatus { get; set; }
    }
}
