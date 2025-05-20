using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.BusinessModel.Transaction;

public record TransactionsWithStatisticsModel
{
    public PagedResult<TransactionModel> Transactions { get; set; } = new();
    public TransactionStatisticsModel Statistics { get; set; } = new();
}