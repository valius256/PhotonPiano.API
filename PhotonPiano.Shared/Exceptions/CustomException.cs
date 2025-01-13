namespace PhotonPiano.Shared.Exceptions;

public class CustomException : ApplicationException
{
    public CustomException(string message, int code) : base(message)
    {
        Code = code;
    }

    public int Code { get; init; }
}