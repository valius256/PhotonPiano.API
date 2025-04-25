using PhotonPiano.BusinessLogic.BusinessModel.Class;

namespace PhotonPiano.BusinessLogic.BusinessModel.StudentScore;

public class StudentDetailedScoreViewModel
{
    public string StudentId { get; set; }
    public string StudentName { get; set; }
    public Guid StudentClassId { get; set; }
    public Guid ClassId { get; set; }
    public string ClassName { get; set; }
    public decimal? Gpa { get; set; }
    public bool? IsPassed { get; set; }
    public List<CriteriaScoreViewModel> CriteriaScores { get; set; }
    public string InstructorComment { get; set; }
    public string CertificateUrl { get; set; }
}