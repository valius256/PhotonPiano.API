namespace PhotonPiano.BusinessLogic.BusinessModel.Class;

public class UpdateStudentScoreModel
{
    public Guid StudentClassId { get; set; }
    public Guid CriteriaId { get; set; }
    public decimal? Score { get; set; }
}