namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTestResult;

public class EntranceTestResultModel
{
    public required Guid Id { get; set; }
    public required Guid EntranceTestStudentId { get; set; }
    public decimal? Score { get; set; }
    public required Guid CriteriaId { get; set; }
    public string? CriteriaName { get; set; }
    public required string CreatedById { get; set; }
    public string? UpdateById { get; set; }
    public string? DeletedById { get; set; }
}