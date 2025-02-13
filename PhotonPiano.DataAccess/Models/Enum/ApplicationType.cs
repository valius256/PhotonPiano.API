namespace PhotonPiano.DataAccess.Models.Enum;

public enum ApplicationType
{
    LeaveOfAbsence, // Đơn tạm nghỉ
    TemporarySuspensionOfTerm, // Đơn tạm hoãn kì
    ReexamineEntranceScore, // Đơn xin phúc tra điểm đầu vào
    ReexamineFinalExamScores, // Đơn xin phúc tra điểm thi
    ClassTransfer, // Đơn xin chuyển lớp
    TeacherComplaint, // Đơn khiếu nại giáo viên
    ServiceComplaint, // Đơn khiếu nại csvc
    Other, // Các loại đơn khác
    CertificateErrorReport // Báo cáo sai sót chứng chỉ
}