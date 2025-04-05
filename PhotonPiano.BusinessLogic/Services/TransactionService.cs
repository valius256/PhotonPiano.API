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
            ? "THANH TOAN LE PHI THI DAU VAO"
            : "THANH TOAN PHI DAY HOC";

        return id.HasValue ? $"[{typeOfTransaction}] [{createDate.Year}/{createDate.Month}] [{id}]"
                : $"[{typeOfTransaction}] [{createDate.Year}/{createDate.Month}]";
    }
}