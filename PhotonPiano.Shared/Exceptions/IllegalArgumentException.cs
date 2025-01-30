namespace PhotonPiano.Shared.Exceptions;

public class IllegalArgumentException : ArgumentException
{
    public IllegalArgumentException(string message) : base(message)
    {
    }
}