
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Criteria
{
    public record BulkUpdateCriteriaModel
    {
        public List<UpdateCriteriaModel> UpdateCriteria { get; init; } = [];
        public required CriteriaFor For { get; init; }
    }

    public record UpdateCriteriaModel
    {
        public required Guid Id { get; init; }
        public string? Name { get; init; }
        public decimal? Weight { get; init; } = 0;
        public string? Description { get; init; }
    }
}
