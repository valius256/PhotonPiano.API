namespace PhotonPiano.DataAccess.Models.Entity;

public class Criteria : BaseEntityWithId
{
    public string Name { get; set; } = string.Empty;
    public decimal Weight { get; set; } = 0;
    public string? Description { get; set; }

    public virtual ICollection<EntranceTestResult> EntranceTestResults { get; set; } = new List<EntranceTestResult>();
}