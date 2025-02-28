namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTestResult;

public record EntranceTestResultModel
{
    public required Guid Id { get; init; }
    public required Guid EntranceTestStudentId { get; init; }
    public decimal? Score { get; init; }
    public required Guid CriteriaId { get; init; }
    public string? CriteriaName { get; init; }
    public required string CreatedById { get; init; }
    public string? UpdateById { get; init; }
    public string? DeletedById { get; init; }
    public decimal? Weight { get; init; }
}