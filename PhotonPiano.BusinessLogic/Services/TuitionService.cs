using Microsoft.EntityFrameworkCore;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.Payment;
using PhotonPiano.BusinessLogic.BusinessModel.Tuition;
using PhotonPiano.BusinessLogic.BusinessModel.Tution;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;
using PhotonPiano.Shared.Exceptions;
using SemaphoreSlim = System.Threading.SemaphoreSlim;

namespace PhotonPiano.BusinessLogic.Services;

public class TuitionService : ITuitionService
{
    private readonly IServiceFactory _serviceFactory;
    private readonly IUnitOfWork _unitOfWork;

    public TuitionService(IUnitOfWork unitOfWork, IServiceFactory serviceFactory)
    {
        _unitOfWork = unitOfWork;
        _serviceFactory = serviceFactory;
    }

    public async Task<string> PayTuition(AccountModel currentAccount, Guid tuitionId, string returnUrl,
        string ipAddress,
        string apiBaseUrl)
    {
        var paymentTuition = await _unitOfWork.TuitionRepository.FindSingleProjectedAsync<Tuition>(
            expression: x => x.Id == tuitionId,
            hasTrackings: false,
            ignoreQueryFilters: false);

        ValidateTuition(paymentTuition, currentAccount.AccountFirebaseId);
        
        var currentYear = DateTime.UtcNow.Year;

        var currentTaxRateConfig = await _serviceFactory.SystemConfigService.GetTaxesRateConfig(currentYear);

        var currentTaxRate = double.TryParse(currentTaxRateConfig?.ConfigValue, out var taxRate) ? taxRate : 0;

        var currentTaxAmount = paymentTuition!.Amount * (decimal)currentTaxRate;
        
        var transactionId = Guid.NewGuid();

        var transaction = new Transaction
        {
            Id = transactionId,
            TransactionCode = _serviceFactory.TransactionService.GetTransactionCode(TransactionType.TutionFee, DateTime.UtcNow,
                transactionId),
            TutionId = tuitionId,
            TaxRate = currentTaxRate,
            TaxAmount = currentTaxAmount,
            Amount = paymentTuition!.Amount + currentTaxAmount,
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
            $"{apiBaseUrl}/api/tuitions/{currentAccount.AccountFirebaseId}/tuition-payment-callback?url={returnUrl}";

        return _serviceFactory.PaymentService.CreateVnPayPaymentUrl(transaction, ipAddress, apiBaseUrl,
            currentAccount.AccountFirebaseId,
            returnUrl, customReturnUrl);
    }

    public async Task HandleTuitionPaymentCallback(VnPayCallbackModel callbackModel, string accountId)
    {
        var transactionCode = Guid.Parse(callbackModel.VnpTxnRef);

        var transaction =
            await _unitOfWork.TransactionRepository.FindSingleProjectedAsync<Transaction>(t => t.Id == transactionCode);

        if (transaction is null || transaction.Id == Guid.Empty) throw new PaymentRequiredException("Payment is required");

        ValidateTransaction(transaction);

        var account = await _unitOfWork.AccountRepository.FindSingleAsync(a => a.AccountFirebaseId == accountId);

        if (account is null) throw new NotFoundException("Account not found");

        await using var dbTransaction = await _unitOfWork.BeginTransactionAsync();

        try
        {
            transaction.PaymentStatus =
                callbackModel.VnpResponseCode == "00" ? PaymentStatus.Succeed : PaymentStatus.Failed;
            transaction.TransactionCode = callbackModel.VnpTransactionNo;
            transaction.UpdatedAt = DateTime.UtcNow;

            switch (transaction.PaymentStatus)
            {
                case PaymentStatus.Succeed:
                    var tutionEntity =
                        await _unitOfWork.TuitionRepository.FindFirstAsync(x => x.Id == transaction.TutionId);

                    tutionEntity!.PaymentStatus = PaymentStatus.Succeed;

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


    public async Task<decimal> GetTuitionRefundAmount(string studentId, Guid? classId)
    {
        var currentTime = DateTime.UtcNow.AddHours(7);
        var currentStudentClass = await _unitOfWork.StudentClassRepository.FindSingleAsync(sc =>
            sc.StudentFirebaseId == studentId && sc.ClassId == classId);

        if (currentStudentClass is null) throw new NotFoundException("Student is not belongs to this class ");

        var currentTuitionHasPaid = await _unitOfWork.TuitionRepository.FindSingleAsync(t =>
            t.StudentClassId == currentStudentClass.Id && t.PaymentStatus == PaymentStatus.Succeed);
        if (currentTuitionHasPaid is null) throw new NotFoundException("This student has not paid for this class yet.");

        var allSlotsInClass =
            await _unitOfWork.SlotRepository.FindProjectedAsync<Slot>(s => s.ClassId == currentStudentClass.ClassId);

        var slotsInCurrentMonth = allSlotsInClass
            .Where(s => s.Date.Month == currentTime.Month && s.Date.Year == currentTime.Year)
            .ToList();

        var numOfSlotHaveAttended = slotsInCurrentMonth
            .Count(slot => slot.SlotStudents.Any(ss =>
                ss.StudentFirebaseId == studentId && ss.AttendanceStatus == AttendanceStatus.Attended));


        var level = await _unitOfWork.LevelRepository.FindSingleAsync(l => l.Id == currentStudentClass.Class.LevelId);
        if (level is null)
        {
            throw new NotFoundException("Level not found");
        }

        var refundAmount = level.PricePerSlot * numOfSlotHaveAttended;
        return currentTuitionHasPaid.Amount - refundAmount;
    }


    // note: this function just run in 1st of month
    public async Task CronAutoCreateTuition()
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
            await _unitOfWork.ClassRepository.FindProjectedAsync<Class>(c =>
                c.Status == ClassStatus.Ongoing && c.IsPublic == true);
        var ongoingClassIds = ongoingClasses.Select(x => x.Id).ToList();

        var studentClasses = await _unitOfWork.StudentClassRepository.FindProjectedAsync<StudentClass>(
            sc => ongoingClassIds.Contains(sc.ClassId)
        );

        var tutions = new List<Tuition>();

        foreach (var studentClass in studentClasses)
        {
            var level = await _unitOfWork.LevelRepository.FindSingleProjectedAsync<Level>(l =>
                l.Id == studentClass.Class.LevelId);

            // levels.SingleOrDefault(l => l.Id == studentClass.Class.LevelId) ?? throw new NotFoundException("Level not found");

            var actualSlotsInMonth =
                studentClass.Class.Slots.Count(sl => sl.Date.Year == utcNow.Year && sl.Date.Month == utcNow.Month);

            if (level != null)
            {
                var tution = new Tuition
                {
                    Id = Guid.NewGuid(),
                    StudentClassId = studentClass.Id,
                    StartDate = utcNow,
                    EndDate = endDate,
                    CreatedAt = utcNow,
                    Amount = level.PricePerSlot * actualSlotsInMonth,
                    PaymentStatus = PaymentStatus.Pending
                };

                if (tution.Amount > 0) tutions.Add(tution);
            }
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


    // note: this function will run in 15th of month
    public async Task CronForTuitionReminder()
    {
        var allNotPaidTuition =
            await _unitOfWork.TuitionRepository.FindProjectedAsync<Tuition>(
                t => t.PaymentStatus == PaymentStatus.Pending,
                false);
        foreach (var tuition in allNotPaidTuition)
        {
            var studentClass =
                await _unitOfWork.StudentClassRepository.FindSingleAsync(sc => sc.Id == tuition.StudentClassId);
            var emailParam = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "studentName", studentClass.Student.Email },
                { "className", studentClass.Class.Name },
                { "startDate", $"{tuition.StartDate:yyyy-MM-dd}" },
                { "endDate", $"{tuition.EndDate:yyyy-MM-dd}" },
                { "amount", $"{tuition.Amount}" }
            };

            await _serviceFactory.EmailService.SendAsync(
                "NotifyTuitionReminder",
                new List<string> { studentClass.Student.Email },
                null,
                emailParam
            );

            await _serviceFactory.NotificationService.SendNotificationAsync(studentClass.Student.AccountFirebaseId,
                $"Học phí tháng {tuition.StartDate:MM/yyyy} của lớp {studentClass.Class.Name} chưa thanh toán",
                "Hãy thanh toán học phí để tiếp tục học tập");
        }
    }

    public async Task CronForTuitionOverdue()
    {
        var overdueTuitions = await _unitOfWork.TuitionRepository.FindProjectedAsync<Tuition>(
            t => t.PaymentStatus == PaymentStatus.Pending,
            false);

        foreach (var tuition in overdueTuitions)
        {
            var studentClass =
                await _unitOfWork.StudentClassRepository.Entities
                .Include(sc => sc.Student)
                .Include(sc => sc.Class)
                .SingleOrDefaultAsync(sc => sc.Id == tuition.StudentClassId);

            if (studentClass is null) continue;

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _unitOfWork.SlotStudentRepository.ExecuteDeleteAsync(
                    ss => ss.StudentFirebaseId == studentClass!.StudentFirebaseId
                );

                await _unitOfWork.StudentClassRepository.ExecuteDeleteAsync(
                    sc => sc.StudentFirebaseId == studentClass!.StudentFirebaseId
                );

                await _unitOfWork.AccountRepository.ExecuteUpdateAsync(
                    account => account.AccountFirebaseId == studentClass!.StudentFirebaseId,
                    account => account.SetProperty(a => a.StudentStatus, StudentStatus.DropOut)
                );
            });

            var emailParam = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "studentName", studentClass.Student.Email },
                { "className", studentClass.Class.Name },
                { "dueDate", tuition.EndDate.ToString("dd-MM-yyyy") },
                { "amount", $"{tuition.Amount}" }
            };

            await _serviceFactory.EmailService.SendAsync(
                "NotifyTuitionOverdue",
                new List<string> { studentClass.Student.Email },
                null,
                emailParam
            );

            await _serviceFactory.NotificationService.SendNotificationAsync(studentClass.Student.AccountFirebaseId,
                $"Học phí tháng {tuition.StartDate:MM/yyyy} của lớp {studentClass.Class.Name} đã quá hạn bạn đã ",
                "Hãy thanh toán học phí để tránh bị gián đoạn học tập");
        }
    }


    public async Task<PagedResult<TuitionWithStudentClassModel>> GetTuitionsPaged(QueryTuitionModel queryTuitionModel,
        AccountModel? account = default)
    {
        var (page, pageSize, sortColumn, orderByDesc, studentClassIds, startDate, endDate, paymentStatuses) =
            queryTuitionModel;

        // Convert DateOnly to DateTime (UTC) for filtering
        var startDateTimeUtc = startDate.HasValue
            ? DateTime.SpecifyKind(startDate.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc)
            : (DateTime?)null;

        var endDateTimeUtc = endDate.HasValue
            ? DateTime.SpecifyKind(endDate.Value.ToDateTime(TimeOnly.MaxValue), DateTimeKind.Utc)
            : (DateTime?)null;

        var result = await _unitOfWork.TuitionRepository.GetPaginatedWithProjectionAsync<TuitionWithStudentClassModel>(
            page, pageSize, sortColumn, orderByDesc,
            expressions:
            [
                x => studentClassIds == null || studentClassIds.Count == 0 ||
                     studentClassIds.Contains(x.StudentClassId),
                x => paymentStatuses == null || paymentStatuses.Count == 0 || paymentStatuses.Contains(x.PaymentStatus),
                x => !startDateTimeUtc.HasValue || x.StartDate <= endDateTimeUtc,
                x => !endDateTimeUtc.HasValue || x.EndDate >= startDateTimeUtc,
                x => account != null && (account.Role == Role.Staff ||
                                         x.StudentClass.StudentFirebaseId == account.AccountFirebaseId)
            ]);

        return result;
    }


    public async Task<TuitionWithStudentClassModel> GetTuitionById(Guid tuitionId,
        AccountModel? currentAccount)
    {
        var result = await _unitOfWork.TuitionRepository
            .FindSingleProjectedAsync<TuitionWithStudentClassModel>(e => e.Id == tuitionId, false);
        
        if (result is null || result.Id == null) throw new NotFoundException("Tuition not found.");
        
        if (result.StudentClass.StudentFirebaseId != currentAccount!.AccountFirebaseId && currentAccount.Role != Role.Staff)
        {
            throw new ForbiddenMethodException("You are not allowed to see this tuition.");
        }

        return result;
    }


    public async Task CreateTuitionWhenRegisterClass(ClassDetailModel classDetailModel)
    {
        var utcNow = DateTime.UtcNow.AddHours(7);
        var utcNowConvert = DateOnly.FromDateTime(utcNow);
        var lastDayOfMonth = DateTime.DaysInMonth(utcNow.Year, utcNow.Month);
        var endDate = new DateTime(utcNow.Year, utcNow.Month, lastDayOfMonth, 23, 59, 59, DateTimeKind.Utc);

        var tuitions = new List<Tuition>();
        foreach (var studentClass in classDetailModel.StudentClasses)
        {
            var numOfSlotTillEndMonth =
                classDetailModel.Slots.Count(x =>
                    x.Date.Month == utcNowConvert.Month && x.ClassId == studentClass.ClassId);
            if (classDetailModel.Level != null)
            {
                var tuition = new Tuition
                {
                    Id = Guid.NewGuid(),
                    StudentClassId = studentClass.Id,
                    StartDate = utcNow,
                    EndDate = endDate,
                    CreatedAt = utcNow,
                    Amount = classDetailModel.Level.PricePerSlot * numOfSlotTillEndMonth,
                    PaymentStatus = PaymentStatus.Pending
                };

                if (tuition.Amount > 0) tuitions.Add(tuition);
            }
        }


        tuitions.AddRange(tuitions);

        await _unitOfWork.TuitionRepository.AddRangeAsync(tuitions);

        // Send emails in parallel
        var emailTasks = tuitions.Select(async result =>
        {
            var student = classDetailModel.StudentClasses.Where(sc => sc.Id == result.StudentClassId).FirstOrDefault();

            if (student != null)
            {
                var emailParam = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "studentName", $"{student.StudentFullName}" },
                { "className", $"{classDetailModel.Name}" },
                { "amount", $"{result.Amount}" },
                { "endDate", $"{result.EndDate:dd/MM/yyyy}" },
                { "startDate", $"{result.StartDate:dd/MM/yyyy}"}
            };

                await _serviceFactory.EmailService.SendAsync("NotifyTuitionCreated",
                    [student.Student.Email, "quangphat7a1@gmail.com"],
                    null, emailParam);
            }

        });

        await Task.WhenAll(emailTasks);
    }


    private void ValidateTransaction(Transaction transaction)
    {
        if (transaction.TutionId is null)
            throw new IllegalArgumentException("The TuitionId of this transaction must not be null.");
        
        if (transaction.PaymentStatus != PaymentStatus.Pending)
            throw new IllegalArgumentException("Payment Status of this transaction must be pending.");
        
        if (transaction.Tution!.PaymentStatus != PaymentStatus.Pending)
            throw new IllegalArgumentException("The PaymentStatus of this tuition must be pending.");
    }

    private void ValidateTuition(Tuition? tuition, string userFirebaseId)
    {
        if (tuition == null)
            throw new NotFoundException("No tuition found for this, please add tuition first.");

        if (tuition.PaymentStatus == PaymentStatus.Succeed)
            throw new BadRequestException("This tuition has already been processed.");

        if (tuition.StudentClass.StudentFirebaseId != userFirebaseId)
            throw new BadRequestException("You are not allowed to pay this tuition.");
    }
}