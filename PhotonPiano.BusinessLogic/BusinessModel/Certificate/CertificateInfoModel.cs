namespace PhotonPiano.BusinessLogic.BusinessModel.Certificate;

public class CertificateInfoModel
{
    public Guid StudentClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string LevelName { get; set; } = string.Empty;
    public DateTime CompletionDate { get; set; }
    public string? CertificateUrl { get; set; }
        
    public string? CertificateHtml { get; set; }
    
    public bool HasCertificateHtml { get; set; }
    public decimal GPA { get; set; }
    
    // Additional properties for detailed view
    public string StudentName { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public List<string> SkillsEarned { get; set; } = new List<string>();
}
