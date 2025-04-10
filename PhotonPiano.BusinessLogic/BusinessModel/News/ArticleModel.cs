namespace PhotonPiano.BusinessLogic.BusinessModel.News;

public record ArticleModel : BaseModel
{
    public required Guid Id { get; init; }
    
    public required string Title { get; init; }
    
    public required string Content { get; init; }

    public required string Slug { get; init; }
    
    public string? Thumbnail { get; init; }
    
    public bool IsPublished { get; init; }
    
    public DateTime? PublishedAt { get; init; }
}