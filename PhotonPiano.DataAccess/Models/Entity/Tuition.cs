using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.Models.Entity;

public class Tuition : BaseEntityWithId
{
    public required Guid StudentClassId { get; set; }
    public decimal Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public PaymentStatus PaymentStatus { get; set; }


    // reference
    public virtual StudentClass StudentClass { get; set; } = default!;
    public virtual ICollection<Transaction> TransactionTuitions { get; set; } = new List<Transaction>();
}