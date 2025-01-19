namespace PhotonPiano.DataAccess.Models.Entity;

public class DayOff : BaseEntityWithId
{
    public string? Name { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public required string CreatedById { get; set; }
    public string? UpdateById { get; set; }
    public string? DeletedById { get; set; }

    // reference 
    public virtual Account CreatedBy { get; set; }
    public virtual Account UpdateBy { get; set; }
    public virtual Account DeletedBy { get; set; }
}