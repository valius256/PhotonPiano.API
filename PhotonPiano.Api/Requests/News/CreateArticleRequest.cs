using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.News;

public record CreateArticleRequest
{
    [Required(ErrorMessage = "Title is required")]
    public required string Title { get; init; }
    
    [Required(ErrorMessage = "Content is required")]
    public required string Content { get; init; }
    
    public string? Thumbnail { get; init; }
}