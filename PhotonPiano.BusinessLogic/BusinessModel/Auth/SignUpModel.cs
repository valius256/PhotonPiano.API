namespace PhotonPiano.BusinessLogic.BusinessModel.Auth;

public record SignUpModel
{
    public required string Email { get; init; }
    public required string Password { get; init; }

    public required string Phone { get; init; }

    public required string DesiredLevel { get; init; }
    
    public required List<string> DesiredTargets { get; init; } 
    
    public required List<string> FavoriteMusicGenres { get; init; } 

    public required List<string> PreferredLearningMethods { get; init; }

    public void Deconstruct(out string email, out string password)
    {
        email = Email;
        password = Password;
    }
}