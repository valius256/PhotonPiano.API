using PhotonPiano.BusinessLogic.BusinessModel.Class;

namespace PhotonPiano.Api.Requests.StudentScore;

public record UpdateBatchStudentScoreRequest
{
    public Guid ClassId { get; set; }
    public List<StudentClassScoreRequest> Scores { get; set; } = new List<StudentClassScoreRequest>();
}