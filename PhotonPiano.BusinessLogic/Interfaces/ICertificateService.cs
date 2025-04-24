using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Certificate;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ICertificateService
{
    Task<string> GenerateCertificateAsync(Guid studentClassId);
    Task<List<CertificateInfoModel>> GetStudentCertificatesAsync(AccountModel account);
    Task<Dictionary<string, string>> GenerateCertificatesForClassAsync(Guid classId);
    Task<CertificateInfoModel> GetCertificateByIdAsync(Guid studentClassId);

    Task CronAutoGenerateCertificatesAsync(Guid classId);
    // Task<CertificateEligibilityResultModel> CheckCertificateEligibilityAsync(Guid studentClassId);
}