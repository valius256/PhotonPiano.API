namespace PhotonPiano.BusinessLogic.BusinessModel.Class;

public class UpdateBatchStudentClassScoreModel
{
    public Guid ClassId { get; set; }
    public List<ScoreUpdateItem> Scores { get; set; } = new List<ScoreUpdateItem>();
}