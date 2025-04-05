using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Criteria;

public record QueryMinimalCriteriasModel
{
    public CriteriaFor CriteriaFor { get; init; }
}