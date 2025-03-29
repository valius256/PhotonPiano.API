using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Criteria;

public record CriteriaGradeModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Weight { get; init; }
    public CriteriaFor For { get; init; }
}