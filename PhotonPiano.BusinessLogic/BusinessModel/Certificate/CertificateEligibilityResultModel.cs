namespace PhotonPiano.BusinessLogic.BusinessModel.Certificate;

public class CertificateEligibilityResultModel
{
    public bool IsEligible { get; set; }
    public string Reason { get; set; }
    
    // Detailed validation results
    public bool ClassCompleted { get; set; }
    public bool GradePassed { get; set; }
    public bool AttendanceSufficient { get; set; }
    public bool TuitionFullyPaid { get; set; }
    
    // Numerical values
    public decimal? AttendancePercentage { get; set; }
    public decimal? AttendanceMinimumRequired { get; set; }
    public decimal? TuitionPaid { get; set; }
    public decimal? TuitionTotal { get; set; }
}