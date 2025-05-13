using PhotonPiano.Api.Requests.Query;

namespace PhotonPiano.Api.Requests.Class;

public record GetAvailableTeacherForClassRequest : QueryPagedRequest
{
    public Guid ClassId { get; init; }
}