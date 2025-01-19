namespace PhotonPiano.DataAccess.Models.Entity;

public class New : BaseEntityWithId
{
    public required string Title { get; set; }
    public string? Content { get; set; }
    public string? Thumbnail { get; set; }

    public required string CreatedById { get; set; }
    public string? UpdateById { get; set; }

    public string? DeletedById { get; set; }

    // reference
    public virtual required Account CreateBy { get; set; } = default!;

    public virtual required Account UpdateBy { get; set; } = default!;

    public virtual required Account DeletedBy { get; set; } = default!;
}