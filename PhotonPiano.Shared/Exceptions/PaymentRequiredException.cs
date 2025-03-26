namespace PhotonPiano.Shared.Exceptions;

public class PaymentRequiredException : ApplicationException
{
    public PaymentRequiredException(string message) : base(message)
    {

    }
}