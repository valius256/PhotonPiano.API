namespace PhotonPiano.BusinessLogic.BusinessModel.News;

public record CreateArticleModel
{
    public required string Title { get; init; }
    
    public required string Content { get; init; }
    
    public string? Thumbnail { get; init; }
    
    public bool IsPublished { get; init; }
}