using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Transaction;
using PhotonPiano.DataAccess.Models.Paging;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ITransactionService
{
    Task<PagedResult<TransactionModel>> GetPagedTransactions(QueryPagedTransactionsModel queryModel, AccountModel currentAccount);
}