namespace PhotonPiano.BusinessLogic.BusinessModel.Application;

public record SendRefundApplicationModel : SendApplicationModel
{
    public required string BankName { get; init; }
    public required string BankAccountName { get; init; }
    public required string BankAccountNumber { get; init; }
}