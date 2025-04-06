using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Criteria;

public record CriteriaModel
{
    public required Guid Id { get; set; }
    public string? Name { get; set; }
    public decimal Weight { get; set; }
    public string? Description { get; set; }

    public string? CreatedById { get; set; }
    public string? UpdateById { get; set; }
    public string? DeletedById { get; set; }
    public CriteriaFor For { get; set; }

}