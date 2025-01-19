namespace PhotonPiano.DataAccess.Models.Entity;

public class StudentClassScore : BaseEntityWithId
{
    public required Guid StudentClassId { get; set; }
    public decimal Score { get; set; }
    public Guid CriteriaId { get; set; }


    // reference 
    public virtual StudentClass StudentClass { get; set; } = default!;
    public virtual Criteria Criteria { get; set; } = default!;
}