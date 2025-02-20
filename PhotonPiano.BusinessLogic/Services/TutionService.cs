using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Payment;
using PhotonPiano.BusinessLogic.BusinessModel.Tution;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;
using PhotonPiano.Shared.Exceptions;
using SemaphoreSlim = System.Threading.SemaphoreSlim;

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
            Amount = paymentTution!.Amount,
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

                    await _serviceFactory.EmailService.SendAsync("PaymentSuccess",
                        new List<string> { account.Email },
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


    // note: this function just run in 1st of month
    public async Task CronAutoCreateTution()
    {
        var utcNow = DateTime.UtcNow.AddHours(7);
        var lastDayOfMonth = DateTime.DaysInMonth(utcNow.Year, utcNow.Month);
        var endDate = new DateTime(utcNow.Year, utcNow.Month, lastDayOfMonth, 23, 59, 59, DateTimeKind.Utc);
        var endDateConverted = DateOnly.FromDateTime(endDate);

        // validate if created tuition or not 
        var isExist = await _unitOfWork.TuitionRepository.AnyAsync(x => x.StartDate == utcNow && x.EndDate == endDate);
        if (isExist) return;

        // Get all ongoing classes
        var ongoingClasses =
            await _unitOfWork.ClassRepository.FindProjectedAsync<Class>(c => c.Status == ClassStatus.Ongoing);
        var ongoingClassIds = ongoingClasses.Select(x => x.Id).ToList();

        var studentClasses = await _unitOfWork.StudentClassRepository.FindProjectedAsync<StudentClass>(
            sc => ongoingClassIds.Contains(sc.ClassId)
        );

        var tutions = new List<Tution>();

        foreach (var studentClass in studentClasses)
        {
            var levelValue = (int)studentClass.Class.Level;
            var systemConfigLevel = await _serviceFactory.SystemConfigService
                .GetSystemConfigValueBaseOnLevel(levelValue);

            var actualSlotsInMonth =
                studentClass.Class.Slots.Count(sl => sl.Date.Year == utcNow.Year && sl.Date.Month == utcNow.Month);

            var tution = new Tution
            {
                Id = Guid.NewGuid(),
                StudentClassId = studentClass.Id,
                StartDate = utcNow,
                EndDate = endDate,
                CreatedAt = utcNow,
                Amount = systemConfigLevel.PriceOfSlot * actualSlotsInMonth,
                PaymentStatus = PaymentStatus.Pending
            };

            if (tution.Amount > 0) tutions.Add(tution);
        }

        var currentTuition = tutions.Count();


        // Save to database inside a transaction
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.TuitionRepository.AddRangeAsync(tutions);
            await _unitOfWork.SaveChangesAsync();
        });


        // Send emails in parallel
        var semaphore = new SemaphoreSlim(10); // Max 10 concurrent emails
        var emailTasks = tutions.Select(async tution =>
        {
            await semaphore.WaitAsync();
            try
            {
                var studentClass = studentClasses.First(sc => sc.Id == tution.StudentClassId);

                var emailParam = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "studentName", studentClass.Student.Email },
                    { "className", studentClass.Class.Name },
                    { "startDate", $"{utcNow:yyyy-MM-dd}" },
                    { "endDate", $"{endDateConverted}" },
                    { "amount", $"{tution.Amount}" }
                };

                await _serviceFactory.EmailService.SendAsync(
                    "NotifyTuitionCreated",
                    new List<string> { studentClass.Student.Email },
                    null,
                    emailParam
                );
            }
            finally
            {
                semaphore.Release(); // Release the semaphore after sending each email
            }
        });

        await Task.WhenAll(emailTasks);
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
                x => studentClassIds == null || studentClassIds.Count == 0 ||
                     studentClassIds.Contains(x.StudentClassId),
                x => paymentStatuses == null || paymentStatuses.Count == 0 || paymentStatuses.Contains(x.PaymentStatus),
                x => !startDate.HasValue || x.StartDate >= startDate.Value,
                x => !endDate.HasValue || x.EndDate <= endDate.Value,
                x => account != null && (account.Role == Role.Staff ||
                                         x.StudentClass.StudentFirebaseId == account.AccountFirebaseId)
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


    public async Task CreateTutionWhenRegisterClass(List<StudentClass> models)
    {
        var utcNow = DateTime.UtcNow.AddHours(7);
        var utcNowConvert = DateOnly.FromDateTime(utcNow);
        var lastDayOfMonth = DateTime.DaysInMonth(utcNow.Year, utcNow.Month);
        var endDate = new DateTime(utcNow.Year, utcNow.Month, lastDayOfMonth, 23, 59, 59, DateTimeKind.Utc);
        var enDateConvert = DateOnly.FromDateTime(endDate);

        var tuitions = new List<Tution>();
        var tasks = models.Select(async studentClass =>
        {
            var sysConfigValue = await _serviceFactory.SystemConfigService
                .GetSystemConfigValueBaseOnLevel((int)studentClass.Class.Level);

            var numOfSlotTillEndMonth =
                studentClass.Class.Slots.Count(x => x.Date >= utcNowConvert && x.Date <= enDateConvert);

            var tuition = new Tution
            {
                Id = Guid.NewGuid(),
                StudentClassId = studentClass.Id,
                StartDate = utcNow,
                EndDate = endDate,
                CreatedAt = utcNow,
                Amount = sysConfigValue.PriceOfSlot * numOfSlotTillEndMonth,
                PaymentStatus = PaymentStatus.Pending
            };

            return (tuition, studentClass.Student.Email, studentClass.Class.Name);
        });

        var tuitionResults = await Task.WhenAll(tasks);

        tuitions.AddRange(tuitionResults.Select(x => x.tuition));

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.TuitionRepository.AddRangeAsync(tuitions);
        });

        // Send emails in parallel
        var emailTasks = tuitionResults.Select(async result =>
        {
            var emailParam = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "customerName", $"{result.Email}" },
                { "className", $"{result.Name}" },
                { "amount", $"{result.tuition.Amount}" },
                { "paymentStatus", $"{result.tuition.PaymentStatus}" },
                { "validFromTo", $"{result.tuition.StartDate:yyyy-MM-dd} to {result.tuition.EndDate:yyyy-MM-dd}" }
            };

            await _serviceFactory.EmailService.SendAsync("PaymentSuccess",
                new List<string> { result.Email, "quangphat7a1@gmail.com" },
                null, emailParam);
        });

        await Task.WhenAll(emailTasks);
    }


    private void ValidateTransaction(Transaction transaction)
    {
        if (transaction.PaymentStatus != PaymentStatus.Pending)
            throw new IllegalArgumentException("Payment Status of this transaction must be pending.");

        if (transaction.TutionId is null)
            throw new IllegalArgumentException("The TuitionId of this transaction must not be null.");

        if (transaction.Tution!.PaymentStatus != PaymentStatus.Pending)
            throw new IllegalArgumentException("The PaymentStatus of this tuition must be pending.");
    }

    private void ValidateTution(Tution? tution, string userFirebaseId)
    {
        if (tution == null)
            throw new NullReferenceException("No tuition found for this, please add tuition first.");

        if (tution.PaymentStatus == PaymentStatus.Successed)
            throw new BadRequestException("This tuition has already been processed.");

        if (tution.StudentClass.StudentFirebaseId != userFirebaseId)
            throw new BadRequestException("You are not allowed to pay this tuition.");
    }
}