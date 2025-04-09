namespace PhotonPiano.DataAccess.Models.Entity;

public class Article : BaseEntityWithId
{
    public required string Title { get; set; }
    
    public required string Content { get; set; }

    public required string Slug { get; set; }
    
    public string? Thumbnail { get; set; }

    public bool IsPublished { get; set; }

    public DateTime? PublishedAt { get; set; }
    public required string CreatedById { get; set; }
    public string? UpdateById { get; set; }
    public string? DeletedById { get; set; }

    // reference
    public virtual Account CreatedBy { get; set; } = default!;
    public virtual Account UpdatedBy { get; set; } = default!;
    public virtual Account DeletedBy { get; set; } = default!;
}