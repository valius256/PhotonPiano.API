namespace PhotonPiano.BusinessLogic.BusinessModel.Certificate;

public class CertificateInfoModel
{
    public Guid StudentClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string LevelName { get; set; } = string.Empty;
    public DateTime CompletionDate { get; set; }
    public string CertificateUrl { get; set; } = string.Empty;
    public decimal GPA { get; set; }
}