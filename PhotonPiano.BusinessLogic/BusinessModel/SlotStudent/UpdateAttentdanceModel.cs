using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.SlotStudent;

public record UpdateAttentdanceModel
{
    public required Guid SlotId { get; init; }
    public List<SlotStudentInfoModel> SlotStudentInfoRequests { get; init; } = [];
}

public record SlotStudentInfoModel : TutorModel
{
    public required string StudentId { get; init; }
    public string? AttendanceComment  { get; init; }
    public AttendanceStatus AttendanceStatus { get; init; }
}

public record TutorModel
{
    public string? GestureComment { get; init; }
    public string? GestureUrl { get; init; }
    public string? FingerNoteComment { get; init; }
    public string? PedalComment { get; init; }
}