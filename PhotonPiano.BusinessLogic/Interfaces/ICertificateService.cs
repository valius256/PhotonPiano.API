namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ICertificateService
{
    Task<string> GenerateCertificateAsync(Guid studentClassId);
}