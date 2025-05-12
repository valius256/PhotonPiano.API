using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Auth;

public record SignUpModel
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    
    public required string FullName { get; init; }

    public required string Phone { get; init; }

    public required Gender Gender { get; init; }

    public Guid? SelfEvaluatedLevelId { get; init; }

    public void Deconstruct(out string email, out string password)
    {
        email = Email;
        password = Password;
    }

    public void Deconstruct(out string email, out string password, out string fullName, out string phone)
    {
        email = Email;
        password = Password;
        fullName = FullName;
        phone = Phone;
    }
}