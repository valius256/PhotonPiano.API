namespace PhotonPiano.BusinessLogic.BusinessModel.Class;

public class CriteriaScoreViewModel
{
    public Guid CriteriaId { get; set; }
    public string CriteriaName { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public decimal? Score { get; set; }
}