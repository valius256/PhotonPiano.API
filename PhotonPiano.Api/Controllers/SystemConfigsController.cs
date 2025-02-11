using Microsoft.AspNetCore.Mvc;
using PhotonPiano.BusinessLogic.Interfaces;

namespace PhotonPiano.Api.Controllers;

[ApiController]
[Route("api/system-configs")]
public class SystemConfigsController
{
    private readonly IServiceFactory _serviceFactory;

    public SystemConfigsController(IServiceFactory serviceFactory)
    {
        _serviceFactory = serviceFactory;
    }
}