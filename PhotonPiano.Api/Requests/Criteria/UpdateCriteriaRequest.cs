using PhotonPiano.BusinessLogic.BusinessModel.Criteria;
using PhotonPiano.DataAccess.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.Criteria
{
    public record BulkUpdateCriteriaRequest
    {
        public List<UpdateCriteriaRequest> UpdateCriteria { get; init; } = [];
        public required CriteriaFor For { get; init; }
    }

    public record UpdateCriteriaRequest
    {
        public required Guid Id { get; init; }
        public string? Name { get; init; }
        [Range(0,100)]
        public decimal? Weight { get; init; } = 0;
        public string? Description { get; init; }
    }
}
