namespace PhotonPiano.BusinessLogic.BusinessModel.Certificate;

public class CertificateModel
{
    public string StudentName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string LevelName { get; set; } = string.Empty;
    public DateTime CompletionDate { get; set; }
    public decimal GPA { get; set; }
    public bool IsPassed { get; set; }
    public string CertificateId { get; set; } = string.Empty;
    public List<string> SkillsEarned { get; set; } = new List<string>();
    public string InstructorName { get; set; } = string.Empty;
    public string InstructorSignatureUrl { get; set; } = string.Empty;
}