using PhotonPiano.BusinessLogic.BusinessModel.Class;

namespace PhotonPiano.BusinessLogic.BusinessModel.StudentScore;

public class StudentScoreViewModel
{
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public decimal? Gpa { get; set; }
    public bool IsPassed { get; set; }
    public string? InstructorComment { get; set; }
    public string? CertificateUrl { get; set; }
    public List<CriteriaScoreViewModel> CriteriaScores { get; set; } = new();
}