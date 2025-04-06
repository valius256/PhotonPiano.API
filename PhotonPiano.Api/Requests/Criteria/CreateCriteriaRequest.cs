using PhotonPiano.DataAccess.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.Criteria
{
    public record CreateCriteriaRequest
    {
        public required string Name { get; init; } = string.Empty;
        [Range(0,100)]
        public required decimal Weight { get; init; } = 0;
        public string? Description { get; init; }
        public required CriteriaFor For { get; init; }
    }
}
