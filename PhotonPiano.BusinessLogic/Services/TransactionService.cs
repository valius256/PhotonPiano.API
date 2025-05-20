using System.Linq.Expressions;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Transaction;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;
using Role = PhotonPiano.DataAccess.Models.Enum.Role;

namespace PhotonPiano.BusinessLogic.Services;

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<TransactionModel>> GetPagedTransactions(QueryPagedTransactionsModel queryModel,
        AccountModel currentAccount)
    {
        var (startDate, endDate, code, id, statuses, paymentMethods) = queryModel;

        return await _unitOfWork.TransactionRepository.GetPaginatedWithProjectionAsync<TransactionModel>(
            queryModel.Page,
            queryModel.PageSize,
            queryModel.SortColumn,
            queryModel.OrderByDesc,
            expressions:
            [
                t => !startDate.HasValue || t.CreatedAt >= startDate,
                t => !endDate.HasValue || t.CreatedAt <= endDate,
                t => !id.HasValue || t.Id == id,
                t => string.IsNullOrEmpty(code) || string.IsNullOrEmpty(t.TransactionCode) || t.TransactionCode.ToLower().Contains(code.ToLower()),
                t => statuses.Count == 0 || statuses.Contains(t.PaymentStatus),
                t => paymentMethods.Count == 0 || paymentMethods.Contains(t.PaymentMethod),
                t => currentAccount.Role != Role.Student || t.CreatedById == currentAccount.AccountFirebaseId,
            ]
        );
    }

    public string GetTransactionCode(TransactionType type, DateTime createDate, Guid? id = default)
    {
        var typeOfTransaction = type == TransactionType.EntranceTestFee
            ? "ENTRANCE TEST FEE"
            : "TUITION FEE";

        return id.HasValue ? $"[{typeOfTransaction}] [{createDate.Year}/{createDate.Month}] [{id}]"
                : $"[{typeOfTransaction}] [{createDate.Year}/{createDate.Month}]";
    }

    private async Task<TransactionStatisticsModel> GetTransactionStatisticsAsync(QueryPagedTransactionsModel queryModel,
        AccountModel currentAccount)
    {
        var (startDate, endDate, code, id, statuses, paymentMethods) = queryModel;
    
        var allTransactions = await _unitOfWork.TransactionRepository.FindProjectedAsync<TransactionModel>(
            t => (!startDate.HasValue || t.CreatedAt >= startDate) &&
                 (!endDate.HasValue || t.CreatedAt <= endDate) &&
                 (!id.HasValue || t.Id == id) &&
                 (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(t.TransactionCode) || t.TransactionCode.ToLower().Contains(code.ToLower())) &&
                 (paymentMethods.Count == 0 || paymentMethods.Contains(t.PaymentMethod)) &&
                 (currentAccount.Role != Role.Student || t.CreatedById == currentAccount.AccountFirebaseId),
            hasTrackings: false
        );

        var result = new TransactionStatisticsModel
        {
            TotalTransactions = allTransactions.Count,
            PendingTransactions = allTransactions.Count(t => t.PaymentStatus == PaymentStatus.Pending),
            CompletedTransactions = allTransactions.Count(t => t.PaymentStatus == PaymentStatus.Succeed),
            FailedTransactions = allTransactions.Count(t => t.PaymentStatus == PaymentStatus.Failed),
            CanceledTransactions = allTransactions.Count(t => t.PaymentStatus == PaymentStatus.Canceled)
        };
    
        return result;
    }
    
    public async Task<TransactionsWithStatisticsModel> GetTransactionsWithStatisticsAsync(
        QueryPagedTransactionsModel queryModel, 
        AccountModel currentAccount)
    {
        var transactions = await GetPagedTransactions(queryModel, currentAccount);
        var statistics = await GetTransactionStatisticsAsync(queryModel, currentAccount);
    
        return new TransactionsWithStatisticsModel
        {
            Transactions = transactions,
            Statistics = statistics
        };  
    }

}