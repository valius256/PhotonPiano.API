using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Application;

public record ApplicationModel
{
    public Guid Id { get; init; }

    public ApplicationType Type { get; init; }

    public ApplicationStatus Status { get; init; } = ApplicationStatus.Pending;

    public required string Reason { get; init; }

    public string? FileUrl { get; init; }

    public string? AdditionalData { get; init; }

    public string? StaffConfirmNote { get; set; }

    // Denormalized  
    public string? CreatedByEmail { get; init; }

    public string? UpdatedByEmail { get; init; }

    public string? DeletedByEmail { get; init; }

    public string? ApprovedByEmail { get; init; }

    public string? CreatedById { get; init; }

    public string? UpdatedById { get; init; }

    public string? ApprovedById { get; init; }

    public DateTime? ApprovedAt { get; init; }

    public string? DeletedById { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; init; }

    public DateTime? DeletedAt { get; init; }

    public RecordStatus RecordStatus { get; init; }
}