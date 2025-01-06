namespace PhotonPiano.DataAccess.Models.Entity;

public class EntranceTestResult : BaseEntityWithId
{
    public required Guid EntranceTestStudentId { get; set; }
    public decimal? Score { get; set; }
    public required Guid CriteriaId { get; set; }
    
    public virtual Criteria Criteria { get; set; }
    public virtual EntranceTestStudent EntranceTestStudent { get; set; }
}