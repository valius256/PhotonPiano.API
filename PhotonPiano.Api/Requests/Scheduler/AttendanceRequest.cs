using PhotonPiano.BusinessLogic.BusinessModel.SlotStudent;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.Scheduler;


public record AttendanceRequest
{
    public required Guid SlotId { get; init; }
    public List<SlotStudentInfoRequest>? SlotStudentInfoRequests { get; init; }
}

public record SlotStudentInfoRequest : TutorModel
{
    public required string StudentId { get; init; }
    public string? AttendanceComment { get; init; }
    public AttendanceStatus AttendanceStatus { get; init; }
}

