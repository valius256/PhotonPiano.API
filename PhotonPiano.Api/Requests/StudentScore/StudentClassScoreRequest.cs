namespace PhotonPiano.Api.Requests.StudentScore;

public class StudentClassScoreRequest
{
    public required Guid StudentClassId { get; set; }
    public decimal? Score { get; set; }
    public Guid CriteriaId { get; set; }
}