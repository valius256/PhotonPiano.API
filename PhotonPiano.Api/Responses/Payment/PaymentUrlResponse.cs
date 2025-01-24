namespace PhotonPiano.Api.Responses.Payment;

public record PaymentUrlResponse
{
    public required string Url { get; init; }
}