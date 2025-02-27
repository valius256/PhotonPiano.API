using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.Models.Entity;

public class Transaction : BaseEntityWithId
{
    public string? Description { get; set; }
    public TransactionType TransactionType { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string? TransactionCode { get; set; }
    public decimal Amount { get; set; }
    public required string CreatedById { get; set; }
    public required string CreatedByEmail { get; set; }
    public Guid? TutionId { get; set; }
    public Guid? EntranceTestStudentId { get; set; }
    public double? TaxRate { get; set; }

    public decimal? TaxAmount { get; set; }

    // Reference
    public virtual Account CreatedBy { get; set; } = default!;
    public virtual Tuition? Tution { get; set; }
    public virtual EntranceTestStudent? EntranceTestStudent { get; set; }
}