using Microsoft.AspNetCore.Http;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IPinataService
{
    Task<string> UploadFile(IFormFile file, string? fileName = default);
}