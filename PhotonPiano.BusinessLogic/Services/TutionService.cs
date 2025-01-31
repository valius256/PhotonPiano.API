using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Payment;
using PhotonPiano.BusinessLogic.BusinessModel.Tution;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services;

public class TutionService : ITutionService
{
    private readonly IServiceFactory _serviceFactory;
    private readonly IUnitOfWork _unitOfWork;

    public TutionService(IUnitOfWork unitOfWork, IServiceFactory serviceFactory)
    {
        _unitOfWork = unitOfWork;
        _serviceFactory = serviceFactory;
    }

    public async Task<string> PayTuition(AccountModel currentAccount, Guid tutionId, string returnUrl, string ipAddress,
        string apiBaseUrl)
    {
        var paymentTution = await _unitOfWork.TuitionRepository.FindFirstProjectedAsync<Tution>(x => x.Id == tutionId);

        ValidateTution(paymentTution, currentAccount.AccountFirebaseId);

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            TutionId = tutionId,
            Amount = paymentTution.Amount,
            CreatedAt = DateTime.UtcNow,
            CreatedById = currentAccount.AccountFirebaseId,
            TransactionType = TransactionType.TutionFee,
            PaymentStatus = PaymentStatus.Pending,
            PaymentMethod = PaymentMethod.VnPay,
            CreatedByEmail = currentAccount.Email
        };

        await _unitOfWork.TransactionRepository.AddAsync(transaction);

        await _unitOfWork.SaveChangesAsync();

        var customReturnUrl =
            $"{apiBaseUrl}/api/tutions/{currentAccount.AccountFirebaseId}/tution-payment-callback?url={returnUrl}";

        return _serviceFactory.PaymentService.CreateVnPayPaymentUrl(transaction, ipAddress, apiBaseUrl,
            currentAccount.AccountFirebaseId,
            returnUrl, customReturnUrl);
    }

    public async Task HandleTutionPaymentCallback(VnPayCallbackModel callbackModel, string accountId)
    {
        var transactionCode = Guid.Parse(callbackModel.VnpTxnRef);

        var transaction =
            await _unitOfWork.TransactionRepository.FindSingleProjectedAsync<Transaction>(t => t.Id == transactionCode);

        if (transaction is null) throw new PaymentRequiredException("Payment is required");

        ValidateTransaction(transaction);

        var account = await _unitOfWork.AccountRepository.FindSingleAsync(a => a.AccountFirebaseId == accountId);

        if (account is null) throw new NotFoundException("Account not found");

        await using var dbTransaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            transaction.PaymentStatus =
                callbackModel.VnpResponseCode == "00" ? PaymentStatus.Successed : PaymentStatus.Failed;
            transaction.TransactionCode = callbackModel.VnpTransactionNo;
            transaction.UpdatedAt = DateTime.UtcNow;

            switch (transaction.PaymentStatus)
            {
                case PaymentStatus.Successed:
                    var tutionEntity =
                        await _unitOfWork.TuitionRepository.FindFirstAsync(x => x.Id == transaction.TutionId);

                    tutionEntity!.PaymentStatus = PaymentStatus.Successed;

                    await _unitOfWork.TransactionRepository.UpdateAsync(transaction);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    var userEmail = account.Email;
                    var emailParam = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        { "customerName", $"{userEmail}" },
                        { "transactionId", $"{transaction.TransactionCode}" },
                        { "amount", $"{transaction.Amount}" },
                        { "orderId", $"{tutionEntity.Id}" },
                        { "paymentMethod", $"{transaction.PaymentMethod}" },
                        { "transactionDate", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") },
                        { "validFromTo", $"{tutionEntity.StartDate:yyyy-MM-dd} to {tutionEntity.EndDate:yyyy-MM-dd}" }
                    };

                    await _serviceFactory.EmailService.SendAsync("PaymentSuccess", new List<string> { account.Email },
                        null, emailParam);


                    break;
                case PaymentStatus.Failed:
                    throw new BadRequestException("Payment has failed.");
                default:
                    throw new BadRequestException("Unknown payment status.");
            }
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task CronAutoCreateTution()
    {
        // Get current time in UTC
        var utcNow = DateTime.UtcNow.AddHours(7);

        var lastDayOfMonth = DateTime.DaysInMonth(utcNow.Year, utcNow.Month);
        var endDate = new DateTime(utcNow.Year, utcNow.Month, lastDayOfMonth, 23, 59, 59, DateTimeKind.Utc);


        var ongoingClass = await _unitOfWork.ClassRepository
            .FindAsync(c => c.Status == ClassStatus.Ongoing);

        var ongoingClassIds = ongoingClass.Select(x => x.Id).ToList();

        var studentClasses = await _unitOfWork.StudentClassRepository
            .FindProjectedAsync<StudentClass>(sc => ongoingClassIds.Contains(sc.ClassId));

        var tutions = new List<Tution>();
        foreach (var studentClass in studentClasses)
        {
            if (studentClass.Class?.Level == null) continue;

            var levelValue = (int)studentClass.Class.Level;
            var systemConfigLevel = await _serviceFactory.SystemConfigService
                .GetSystemConfigValueBaseOnLevel(levelValue);

            tutions.Add(new Tution
            {
                Id = Guid.NewGuid(),
                StudentClassId = studentClass.Id,
                StartDate = utcNow,
                EndDate = endDate,
                CreatedAt = utcNow,
                Amount = systemConfigLevel.PriceOfSlot * systemConfigLevel.NumOfSlotInWeek * 4,
                PaymentStatus = PaymentStatus.Pending
            });
        }

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.TuitionRepository.AddRangeAsync(tutions);
            await _unitOfWork.SaveChangesAsync();
        });

        // Todo:  send email or not how the fuck i know
    }

    public async Task<PagedResult<TutionWithStudentClassModel>> GetTutionsPaged(QueryTutionModel queryTutionModel,
        AccountModel? account = default)
    {
        var (page, pageSize, sortColumn, orderByDesc, studentClassIds, startDate, endDate, paymentStatuses) =
            queryTutionModel;

        var result = await _unitOfWork.TuitionRepository.GetPaginatedWithProjectionAsync<TutionWithStudentClassModel>(
            page, pageSize, sortColumn, orderByDesc,
            expressions:
            [
                x => studentClassIds == null || studentClassIds.Count == 0 || studentClassIds.Contains(x.StudentClassId),
                x => paymentStatuses == null || paymentStatuses.Count == 0 || paymentStatuses.Contains(x.PaymentStatus),
                x => !startDate.HasValue || x.StartDate >= startDate.Value,
                x => !endDate.HasValue || x.EndDate <= endDate.Value,
                x => account != null && (account.Role == Role.Staff || x.StudentClass.StudentFirebaseId == account.AccountFirebaseId)
            ]);


        return result;
    }

    public async Task<TutionWithStudentClassModel> GetTutionById(Guid tutionId)
    {
        
        var result = await _unitOfWork.TuitionRepository
            .FindSingleProjectedAsync<TutionWithStudentClassModel>(e => e.Id == tutionId, false);
        if (result is null) throw new NotFoundException("Tuition not found.");
        
        return result;
    }

    private void ValidateTransaction(Transaction transaction)
    {
        if (transaction.PaymentStatus != PaymentStatus.Pending)
            throw new IllegalArgumentException("Payment Status of this transaction must be pending.");

        if (transaction.TutionId is null)
            throw new IllegalArgumentException("The TutionId of this transaction must not be null.");

        if (transaction.Tution!.PaymentStatus != PaymentStatus.Pending)
            throw new IllegalArgumentException("The PaymentStatus of this tution must be pending.");
    }

    private void ValidateTution(Tution? tution, string userFirebaseId)
    {
        if (tution == null)
            throw new NullReferenceException("No tuition found for this, please add tuition first.");

        if (tution.PaymentStatus == PaymentStatus.Successed)
            throw new BadRequestException("This tution has already been processed.");

        if (tution.StudentClass.StudentFirebaseId != userFirebaseId)
            throw new BadRequestException("You are not allowed to pay this tution.");
    }
}