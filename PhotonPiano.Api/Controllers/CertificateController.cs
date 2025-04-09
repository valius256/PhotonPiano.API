using Mapster;
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.Api.Attributes;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Certificate;
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
    
    [HttpGet("my-certificates")]
    [FirebaseAuthorize(Roles = [Role.Student])]
    [EndpointDescription("Get all certificates for a specific student")]
    public async Task<ActionResult<List<CertificateInfoModel>>> GetMyStudentCertificates()
    {
        var certificates = await _serviceFactory.CertificateService.GetStudentCertificatesAsync(CurrentAccount!);
        return Ok(certificates);
        
        // var pagedResult = await
        //     _serviceFactory.CertificateService.GetStudentCertificatesAsync(Request.Adapt(<>))
    }

    [HttpPost("class/{classId}")]
    [FirebaseAuthorize(Roles = [Role.Staff, Role.Instructor])]
    [EndpointDescription("Generate a certificate for a class")]
    public async Task<ActionResult<Dictionary<string, string>>> GenerateCertificatesForClass([FromRoute] Guid classId)
    {
        var certificates = await _serviceFactory.CertificateService.GenerateCertificatesForClassAsync(classId);
        return Ok(certificates);
    }
}