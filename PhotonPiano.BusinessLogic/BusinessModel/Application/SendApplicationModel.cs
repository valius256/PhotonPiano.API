using Microsoft.AspNetCore.Http;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Application;

public record SendApplicationModel
{
    public required ApplicationType Type { get; init; }
    
    public required string Reason { get; init; }
    
    public IFormFile? File { get; init; }
}