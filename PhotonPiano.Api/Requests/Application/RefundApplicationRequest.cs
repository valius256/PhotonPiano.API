namespace PhotonPiano.Api.Requests.Application;

public record RefundApplicationRequest : SendApplicationRequest
{
    public required string BankName { get; set; }
    public required string BankAccountName { get; set; }
    public required string BankAccountNumber { get; set; }
    public required decimal Amount { get; set; } // số tiền tạm tính
}