using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Certificate;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ICertificateService
{
    Task<(string certificateUrl, Guid studentClassId)> GenerateCertificateAsync(Guid studentClassId);
    Task<List<CertificateInfoModel>> GetStudentCertificatesAsync(AccountModel account);
    Task<Dictionary<string, string>> GenerateCertificatesForClassAsync(Guid classId);
    Task<CertificateInfoModel> GetCertificateByIdAsync(Guid studentClassId);

    Task AutoGenerateCertificatesAsync(Guid classId);

    Task<string> GenerateCertificateAsync(Guid classId, string studentFirebaseId);

    Task<CertificateInfoModel> GetCertificateByClassAndStudentAsync(Guid classId, string studentFirebaseId);
    // Task<CertificateEligibilityResultModel> CheckCertificateEligibilityAsync(Guid studentClassId);
}