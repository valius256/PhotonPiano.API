using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Application;

public record UpdateApplicationModel
{
    public required ApplicationStatus Status { get; init; }
}