namespace PhotonPiano.BusinessLogic.BusinessModel.Auth;

public record CreateSystemAccountModel
{
    public required string Email { get; init; }
    
    public required string FullName { get; init; }

    public required string Phone { get; init; }

    public void Deconstruct(out string email)
    {
        email = Email;
    }

    public void Deconstruct(out string email, out string fullName, out string phone)
    {
        email = Email;
        fullName = FullName;
        phone = Phone;
    }
}