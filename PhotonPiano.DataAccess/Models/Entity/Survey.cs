namespace PhotonPiano.DataAccess.Models.Entity;

public class Survey : BaseEntityWithId
{
    public string Name { get; set; } = default!;
    public required string  CreateById { get; set; }
    public string? UpdateById { get; set; }
    
    // reference
    public virtual Account CreateBy { get; set; } = default!;
    public virtual Account UpdateBy { get; set; } = default!;
}