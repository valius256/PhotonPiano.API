using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Controllers;

[Route("api/certificates")]
[ApiController]
public class CertificateController : BaseController
{
    private readonly IServiceFactory _serviceFactory;

    public CertificateController(IServiceFactory serviceFactory)
    {
        _serviceFactory = serviceFactory;
    }

    [HttpPost("student/{studentClassId}")]
    [FirebaseAuthorize(Roles = [Role.Staff, Role.Instructor])]
    [EndpointDescription("Generate a certificate for a student")]
    public async Task<IActionResult> GenerateCertificate([FromRoute] Guid studentClassId)
    {
        var certificateUrl = await _serviceFactory.CertificateService.GenerateCertificateAsync(studentClassId);
        return Ok(new { url = certificateUrl });
    }
}