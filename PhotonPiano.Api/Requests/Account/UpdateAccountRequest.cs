using System.ComponentModel.DataAnnotations;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.Account;

public record UpdateAccountRequest
{
    public string? UserName { get; init; }
    
    public string? Phone { get; init; } 
    
    [EmailAddress]
    public string? Email { get; init; }

    public string? FullName { get; init; }  
    
    public string? AvatarUrl { get; init; } 
    
    public DateTime? DateOfBirth { get; init; }
    
    public string? Address { get; init; } 
    
    public Gender? Gender { get; init; }
    
    public string? ShortDescription { get; init; } 
}