
using Microsoft.AspNetCore.Mvc;
using PhotonPiano.BusinessLogic.Interfaces;

namespace PhotonPiano.Api.Controllers
{
    public record UploadRequest
    {
        [FromForm(Name = "file")]
        public required IFormFile File { get; init; }
    }
    [Route("api/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IServiceFactory _serviceFactory;

        public FilesController(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<string>> UploadFile([FromForm] UploadRequest request)
        {
            string url = await _serviceFactory.PinataService.UploadFile(request.File);
            return Created(nameof(UploadFile), url);
        }
    }
}
