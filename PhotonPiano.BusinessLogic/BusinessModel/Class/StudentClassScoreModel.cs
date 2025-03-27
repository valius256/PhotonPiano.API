

using PhotonPiano.BusinessLogic.BusinessModel.Criteria;

namespace PhotonPiano.BusinessLogic.BusinessModel.Class
{
    public record StudentClassScoreModel
    {
        public required Guid StudentClassId { get; init; }
        public decimal? Score { get; init; }
        public Guid CriteriaId { get; init; }
        public CriteriaModel Criteria { get; init; } = default!;
    }
}
