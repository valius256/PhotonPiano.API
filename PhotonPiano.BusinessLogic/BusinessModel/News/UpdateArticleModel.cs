namespace PhotonPiano.BusinessLogic.BusinessModel.News;

public record UpdateArticleModel
{
    public string? Title { get; init; }
    
    public string? Content { get; init; }
    
    public string? Thumbnail { get; init; }
}