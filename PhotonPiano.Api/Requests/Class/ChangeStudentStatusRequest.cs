using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.Class;

public class ChangeStudentStatusRequest
{
    public required StudentStatus NewStatus { get; set; }
    public Guid? ClassId { get; set; }
}