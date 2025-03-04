using PhotonPiano.DataAccess.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.Class
{
    public record CreateClassRequest
    {
        [EnumDataType(typeof(Level))]
        public required Level Level { get; init; }
    }
}
