

using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Criteria
{
    public record CreateCriteriaModel
    {
        public required string Name { get; init; } = string.Empty;
        public required decimal Weight { get; init; } = 0;
        public string? Description { get; init; }
        public required CriteriaFor For { get; init; }
    }
}
