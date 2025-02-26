using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Account;

public record UpdateAccountModel
{
    public string? UserName { get; init; }
    
    public string? Phone { get; init; } 
    public string? Email { get; init; }

    public string? FullName { get; init; }
    
    public string? AvatarUrl { get; init; } 
    
    public DateTime? DateOfBirth { get; init; }
    
    public string? Address { get; init; } 
    
    public Gender? Gender { get; init; }
    
    public string? ShortDescription { get; init; } 
}