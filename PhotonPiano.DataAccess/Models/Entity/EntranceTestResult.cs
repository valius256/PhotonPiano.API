namespace PhotonPiano.DataAccess.Models.Entity;

public class EntranceTestResult : BaseEntityWithId
{
    public required Guid EntranceTestStudentId { get; set; }
    public decimal? Score { get; set; }
    public required Guid CriteriaId { get; set; }
    public string? CriteriaName { get; set; }
    public required string CreatedById { get; set; }
    public string? UpdateById { get; set; }
    public string? DeletedById { get; set; }
    public decimal? Weight { get; set; }

    // reference 

    public virtual Account CreatedBy { get; set; } = default!;
    public virtual Account UpdateBy { get; set; } = default!;
    public virtual Account DeletedBy { get; set; } = default!;
    public virtual Criteria Criteria { get; set; } = default!;
    public virtual EntranceTestStudent EntranceTestStudent { get; set; } = default!;
}