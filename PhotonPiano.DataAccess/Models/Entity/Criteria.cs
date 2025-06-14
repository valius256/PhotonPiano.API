using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.Models.Entity;

public class Criteria : BaseEntityWithId
{
    public string Name { get; set; } = string.Empty;
    public decimal Weight { get; set; } = 0;
    public string? Description { get; set; }
    public required string CreatedById { get; set; }
    public string? UpdateById { get; set; }
    public string? DeletedById { get; set; }
    public CriteriaFor For { get; set; }

    // reference 

    public virtual Account CreatedBy { get; set; } = default!;
    public virtual Account UpdateBy { get; set; } = default!;
    public virtual Account DeletedBy { get; set; } = default!;

    public virtual ICollection<EntranceTestResult> EntranceTestResults { get; set; } = new List<EntranceTestResult>();

    public virtual ICollection<StudentClassScore> StudentClassScoreCriterias { get; set; } =
        new List<StudentClassScore>();
}