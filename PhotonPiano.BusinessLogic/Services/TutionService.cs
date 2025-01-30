using PhotonPiano.BusinessLogic.BusinessModel.Payment;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
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


    public async Task<string> PayTuition(string userFirebaseId, Guid tutionId, string returnUrl, string ipAddress,
        string apiBaseUrl)
    {
        var paymentTution = await _unitOfWork.TuitionRepository.FindFirstProjectedAsync<Tution>(x => x.Id == tutionId);

        ValidateTution(paymentTution, userFirebaseId);

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            TutionId = tutionId,
            Amount = paymentTution.Amount,
            CreatedAt = DateTime.UtcNow,
            CreatedById = userFirebaseId,
            TransactionType = TransactionType.TutionFee,
            PaymentStatus = PaymentStatus.Pending,
            PaymentMethod = PaymentMethod.VnPay
        };

        await _unitOfWork.TransactionRepository.AddAsync(transaction);

        await _unitOfWork.SaveChangesAsync();

        var customReturnUrl =
            $"{apiBaseUrl}/api/tutions/{userFirebaseId}/tution-payment-callback?url={returnUrl}";

        return _serviceFactory.PaymentService.CreateVnPayPaymentUrl(transaction, ipAddress, apiBaseUrl,
            userFirebaseId,
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