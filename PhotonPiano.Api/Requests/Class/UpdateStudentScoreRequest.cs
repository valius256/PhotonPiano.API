namespace PhotonPiano.Api.Requests.Class;

public class UpdateStudentScoreRequest
{
    public required Guid StudentClassId { get; set; }
    
    public required Guid CriteriaId { get; set; }
        
    public decimal? Score { get; set; }
}