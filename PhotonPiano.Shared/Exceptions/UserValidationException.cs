namespace PhotonPiano.Shared.Exceptions;

public class UserValidationException : ApplicationException
{
    public UserValidationException(string message) : base(message)
    {
    }
}