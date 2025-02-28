using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.Models.Entity;

public class Application : BaseEntityWithId
{
    public ApplicationType Type { get; set; }
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
    public required string Reason { get; set; }
    public string? FileUrl { get; set; }

    public string? AdditionalData { get; set; }


    // Denormalized  
    public required string CreatedByEmail { get; set; }
    public string? UpdatedByEmail { get; set; }
    public string? DeletedByEmail { get; set; }
    public string? ApprovedByEmail { get; set; }

    public required string CreatedById { get; set; }
    public string? UpdatedById { get; set; }
    public string? ApprovedById { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? DeletedById { get; set; }

    // reference 
    public virtual Account CreatedBy { get; set; } = default!;
    public virtual Account UpdatedBy { get; set; } = default!;
    public virtual Account ApprovedBy { get; set; } = default!;
    public virtual Account DeletedBy { get; set; } = default!;
}