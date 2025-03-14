using PhotonPiano.DataAccess.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.Class
{
    public record CreateClassRequest
    {
        [EnumDataType(typeof(LevelEnum))]
        public required LevelEnum Level { get; init; }
    }
}
