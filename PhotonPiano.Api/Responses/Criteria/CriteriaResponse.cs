using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Responses.Criteria;

public record CriteriaResponse
{
    public required Guid Id { get; init; }

    public string Name { get; init; } = default!;

    public decimal Weight { get; init; }

    public string? Description { get; init; }

    public required string CreatedById { get; init; }

    public string? UpdateById { get; init; }

    public string? DeletedById { get; init; }

    public DateTime CreatedAt { get; init; }


    public DateTime? UpdatedAt { get; init; }

    public DateTime? DeletedAt { get; init; }

    public RecordStatus Status { get; init; }

    public CriteriaFor For { get; init; }
}