using PhotonPiano.DataAccess.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.Class
{
    public record CreateClassRequest
    {
        public required Guid LevelId { get; init; }
    }
}
