namespace PhotonPiano.BusinessLogic.BusinessModel.Application;

public record SendRefundApplicationModel : SendApplicationModel
{
    public required string BankName { get; set; }
    public required string BankAccountName { get; set; }
    public required string BankAccountNumber { get; set; }
    public required decimal Amount { get; set; } // số tiền tạm tính
}