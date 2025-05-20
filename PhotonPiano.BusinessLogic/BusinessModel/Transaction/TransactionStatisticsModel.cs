namespace PhotonPiano.BusinessLogic.BusinessModel.Transaction;

public record TransactionStatisticsModel
{
    public int TotalTransactions { get; set; }
    public int PendingTransactions { get; set; }
    public int CompletedTransactions { get; set; }
    public int FailedTransactions { get; set; }
    public int CanceledTransactions { get; set; }
}