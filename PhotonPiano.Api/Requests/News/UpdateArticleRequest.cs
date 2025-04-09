namespace PhotonPiano.Api.Requests.News;

public record UpdateArticleRequest
{
    public string? Title { get; init; }
    
    public string? Content { get; init; }
    
    public string? Thumbnail { get; init; }
    
    public bool? IsPublished { get; init; }
}