using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Transaction;

public record TransactionModel
{
    public required Guid Id { get; init; }

    public required string Code { get; init; }
    
    public string? Description { get; init; }
    
    public TransactionType TransactionType { get; init; }
    
    public PaymentMethod PaymentMethod { get; init; }
    
    public PaymentStatus PaymentStatus { get; init; }
    
    public string? TransactionCode { get; init; }
    
    public decimal Amount { get; init; }
    
    public required string CreatedById { get; init; }
    
    public required string CreatedByEmail { get; init; }
    
    public Guid? TutionId { get; init; }

    public Guid? EntranceTestStudentId { get; init; }
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow.AddHours(7);
    
    public DateTime? UpdatedAt { get; init; }
    
    public DateTime? DeletedAt { get; init; }
    
    public RecordStatus RecordStatus { get; init; } = RecordStatus.IsActive;
}