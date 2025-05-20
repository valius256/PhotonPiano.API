using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Transaction;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ITransactionService
{
    Task<PagedResult<TransactionModel>> GetPagedTransactions(QueryPagedTransactionsModel queryModel, AccountModel currentAccount);

    string GetTransactionCode(TransactionType type, DateTime createDate, Guid? id = default);

    Task<TransactionsWithStatisticsModel> GetTransactionsWithStatisticsAsync(
        QueryPagedTransactionsModel queryModel,
        AccountModel currentAccount);

}