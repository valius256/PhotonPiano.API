using System.ComponentModel.DataAnnotations;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.Auth;

public record SignUpRequest
{
    [Required] 
    [EmailAddress] 
    public required string Email { get; init; }
    
    [Required] 
    public required string Password { get; init; }
    
    [Required]
    [MinLength(10, ErrorMessage = "Phone number must be at least 10 characters.")]
    public required string Phone { get; init; }

    public required Level Level { get; init; }
    
    public required List<string> DesiredTargets { get; init; } 
    
    public required List<string> FavoriteMusicGenres { get; init; } 

    public required List<string> PreferredLearningMethods { get; init; } 
}