using System.ComponentModel.DataAnnotations;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.Auth;

public record SignUpRequest
{
    [Required(ErrorMessage = "Email is required")] 
    [EmailAddress(ErrorMessage = "Invalid email address")] 
    public required string Email { get; init; }
    
    [Required(ErrorMessage = "Full name is required.")]
    public required string FullName { get; init; }
    
    [Required(ErrorMessage = "Password is required.")] 
    public required string Password { get; init; }
    
    [Required(ErrorMessage = "Phone is reqired")]
    [MinLength(10, ErrorMessage = "Phone number must be at least 10 characters.")]
    public required string Phone { get; init; }
    
    [Required(ErrorMessage = "Level is required.")]
    public required Guid Level { get; init; }
    
    [Required(ErrorMessage = "Targets are required.")]
    public required List<string> DesiredTargets { get; init; } 
    
    [Required(ErrorMessage = "Favorite music genres are required")]
    
    public required List<string> FavoriteMusicGenres { get; init; } 
    
    [Required(ErrorMessage = "Preferred learning methods are required")]
    public required List<string> PreferredLearningMethods { get; init; } 
}