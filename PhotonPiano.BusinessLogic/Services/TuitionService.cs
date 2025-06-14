using System.Globalization;
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
using PhotonPiano.Shared.Utils;
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

    public async Task<string> PayTuition(AccountModel currentAccount, PayTuitionModel model)
    {
        var paymentTuition = await _unitOfWork.TuitionRepository.FindSingleProjectedAsync<Tuition>(
            x => x.Id == model.TuitionId,
            false);

        ValidateTuition(paymentTuition, currentAccount.AccountFirebaseId);

        var currentYear = DateTime.UtcNow.Year;

        var currentTaxRateConfig = await _serviceFactory.SystemConfigService.GetTaxesRateConfig(currentYear);

        var currentTaxRate = double.TryParse(currentTaxRateConfig?.ConfigValue, out var taxRate) ? taxRate : 0;

        var currentTaxAmount = paymentTuition!.Amount * (decimal)currentTaxRate;

        var transactionId = Guid.NewGuid();

        var transaction = new Transaction
        {
            Id = transactionId,
            TransactionCode = _serviceFactory.TransactionService.GetTransactionCode(TransactionType.TutionFee,
                DateTime.UtcNow,
                transactionId),
            TutionId = model.TuitionId,
            TaxRate = currentTaxRate,
            TaxAmount = currentTaxAmount,
            Amount = -1 * (paymentTuition!.Amount + currentTaxAmount),
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
            $"{model.ApiBaseUrl}/api/tuitions/{currentAccount.AccountFirebaseId}/tuition-payment-callback?url={model.ReturnUrl}";

        return _serviceFactory.PaymentService.CreateVnPayPaymentUrl(transaction, model.IpAddress, model.ApiBaseUrl,
            currentAccount.AccountFirebaseId,
            model.ReturnUrl, customReturnUrl);
    }

    public async Task HandleTuitionPaymentCallback(VnPayCallbackModel callbackModel, string accountId)
    {
        var transactionCode = Guid.Parse(callbackModel.VnpTxnRef);

        var transaction =
            await _unitOfWork.TransactionRepository.FindSingleProjectedAsync<Transaction>(t => t.Id == transactionCode);

        if (transaction is null || transaction.Id == Guid.Empty)
            throw new PaymentRequiredException("Payment is required");

        ValidateTransaction(transaction);

        var account = await _unitOfWork.AccountRepository.FindSingleAsync(a => a.AccountFirebaseId == accountId);

        if (account is null)
            throw new NotFoundException("Account not found");

        transaction.PaymentStatus =
            callbackModel.VnpResponseCode == "00" ? PaymentStatus.Succeed : PaymentStatus.Failed;
        transaction.TransactionCode = callbackModel.VnpTransactionNo;
        transaction.UpdatedAt = DateTime.UtcNow;

        if (transaction.PaymentStatus == PaymentStatus.Succeed)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var tuitionEntity =
                    await _unitOfWork.TuitionRepository.FindFirstAsync(x => x.Id == transaction.TutionId);

                if (tuitionEntity is null)
                    throw new NotFoundException($"Tuition with ID {transaction.TutionId} not found");

                tuitionEntity.PaymentStatus = PaymentStatus.Succeed;

                await _unitOfWork.TransactionRepository.UpdateAsync(transaction);
                await _unitOfWork.TuitionRepository.UpdateAsync(tuitionEntity);
            });

            var emailParam = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "customerName", account.Email },
                { "transactionId", transaction.TransactionCode },
                { "amount", transaction.Amount.ToString(CultureInfo.InvariantCulture) },
                { "orderId", transaction.TutionId.ToString() },
                { "paymentMethod", transaction.PaymentMethod.ToString() },
                { "transactionDate", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") },
                {
                    "validFromTo",
                    $"{transaction.Tution?.StartDate:yyyy-MM-dd} to {transaction.Tution?.EndDate:yyyy-MM-dd}"
                }
            };

            await _serviceFactory.EmailService.SendAsync("PaymentSuccess",
                new List<string> { account.Email },
                null, emailParam);

            await _serviceFactory.NotificationService.SendNotificationAsync(account.AccountFirebaseId,
                "Tuition Payment Success",
                "Your tuition payment was successful and has been processed. Thank you for your payment.");
        }
        else if (transaction.PaymentStatus == PaymentStatus.Failed)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var tuitionEntity =
                    await _unitOfWork.TuitionRepository.FindFirstAsync(x => x.Id == transaction.TutionId);

                if (tuitionEntity is null)
                    throw new NotFoundException($"Tuition with ID {transaction.TutionId} not found");
                await _unitOfWork.TransactionRepository.UpdateAsync(transaction);
            });

            var emailParam = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "customerName", account.Email },
                { "transactionId", transaction.TransactionCode },
                { "amount", Math.Abs(transaction.Amount).ToString(CultureInfo.InvariantCulture) },
                { "orderId", transaction.TutionId.ToString() },
                { "paymentMethod", transaction.PaymentMethod.ToString() },
                { "transactionDate", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") },
                { "errorCode", callbackModel.VnpResponseCode },
                { "errorMessage", "Your payment was not successful. Please try again or contact support." }
            };

            await _serviceFactory.EmailService.SendAsync("PaymentFailed",
                new List<string> { account.Email },
                null, emailParam);

            await _serviceFactory.NotificationService.SendNotificationAsync(account.AccountFirebaseId,
                "Tuition Payment Failed",
                "Your tuition payment was not successful. Please try again or contact support.");

            throw new BadRequestException("Payment has failed.");
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
        if (level is null) throw new NotFoundException("Level not found");

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

        var studentClasses =
            await _unitOfWork.StudentClassRepository.FindProjectedAsync<StudentClass>(sc =>
                ongoingClassIds.Contains(sc.ClassId)
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
                    PaymentStatus = PaymentStatus.Pending,
                    Deadline = utcNow.AddDays(4)
                };

                if (tution.Amount > 0) tutions.Add(tution);
            }
        }

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
                    { "startDate", $"{utcNow:dd-MM-yyyy}" },
                    { "endDate", $"{endDateConverted:dd-MM-yyyy}" },
                    { "amount", $"{tution.Amount}" },
                    { "deadline", $"{tution.Deadline:dd-MM-yyyy}" }
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
        var today = DateTime.UtcNow.AddHours(7);

        // Lấy học phí chưa thanh toán, còn thời gian (deadline > hôm nay)
        var upcomingTuitions = await _unitOfWork.TuitionRepository.FindProjectedAsync<Tuition>(
            t => t.PaymentStatus == PaymentStatus.Pending &&
                 t.Deadline > today, // Từ hôm nay đến trước deadline
            false
        );

        foreach (var tuition in upcomingTuitions)
        {
            var studentClass =
                await _unitOfWork.StudentClassRepository.FindSingleProjectedAsync<StudentClass>(
                    sc => sc.Id == tuition.StudentClassId, false);

            var emailParam = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "studentName", studentClass.Student.FullName ?? studentClass.Student.UserName },
                { "className", studentClass.Class.Name },
                { "startDate", $"{tuition.StartDate:dd-MM-yyyy}" },
                { "endDate", $"{tuition.EndDate:dd-MM-yyyy}" },
                { "amount", $"{tuition.Amount:#,##0}" },
                { "deadline", $"{tuition.Deadline:dd-MM-yyyy}" }
            };

            await _serviceFactory.EmailService.SendAsync(
                "NotifyTuitionReminder",
                new List<string> { studentClass.Student.Email },
                null,
                emailParam
            );

            await _serviceFactory.NotificationService.SendNotificationAsync(
                studentClass.Student.AccountFirebaseId,
                $"Monthly tuition {tuition.StartDate:MM/yyyy} of class {studentClass.Class.Name} has not been paid",
                $"Deadline is {tuition.Deadline:dd-MM}. Please pay so your studies will not be interrupted."
            );
        }
    }


    public async Task CronForTuitionOverdue()
    {
        var today = DateTime.UtcNow.AddHours(7);

        var overdueTuitions = await _unitOfWork.TuitionRepository.FindProjectedAsync<Tuition>(
            t => t.PaymentStatus == PaymentStatus.Pending &&
                 t.Deadline < today &&
                 t.IsOverdueProcessed == false,
            false
        );

        foreach (var tuition in overdueTuitions)
        {
            var studentClass = await _unitOfWork.StudentClassRepository.Entities
                .Include(sc => sc.Student)
                .Include(sc => sc.Class)
                .ThenInclude(sc => sc.Slots)
                .SingleOrDefaultAsync(sc => sc.Id == tuition.StudentClassId);

            if (studentClass is null) continue;

            // Xử lý transaction: xoá dữ liệu & cập nhật trạng thái
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _unitOfWork.SlotStudentRepository.ExecuteDeleteAsync(ss =>
                    ss.StudentFirebaseId == studentClass.StudentFirebaseId &&
                    ss.AttendanceStatus == AttendanceStatus.NotYet &&
                    studentClass.Class.Slots.Select(x => x.Id).Contains(ss.SlotId)
                );

                await _unitOfWork.StudentClassRepository.ExecuteDeleteAsync(sc =>
                    sc.StudentFirebaseId == studentClass.StudentFirebaseId &&
                    sc.ClassId == studentClass.ClassId
                );

                await _unitOfWork.AccountRepository.ExecuteUpdateAsync(
                    acc => acc.AccountFirebaseId == studentClass.StudentFirebaseId,
                    acc => acc.SetProperty(a => a.StudentStatus, StudentStatus.DropOut)
                );

                await _unitOfWork.TuitionRepository.ExecuteUpdateAsync(
                    t => t.Id == tuition.Id,
                    t => t.SetProperty(tuition => tuition.IsOverdueProcessed, true)
                );
            });

            var emailParam = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "studentName", studentClass.Student.Email },
                { "className", studentClass.Class.Name },
                { "dueDate", tuition.Deadline.ToString("dd-MM-yyyy") },
                { "amount", $"{tuition.Amount:#,##0}" }
            };

            await _serviceFactory.EmailService.SendAsync(
                "NotifyTuitionOverdue",
                new List<string> { studentClass.Student.Email },
                null,
                emailParam
            );

            await _serviceFactory.NotificationService.SendNotificationAsync(
                studentClass.Student.AccountFirebaseId,
                $"The tuition fee for {tuition.StartDate:MM/yyyy} of class {studentClass.Class.Name} is overdue",
                "You have been removed from the class due to unpaid tuition. Please contact us for assistance."
            );
        }
    }


    public async Task<PagedResult<TuitionWithStudentClassModel>> GetTuitionsPaged(QueryTuitionModel queryTuitionModel,
        AccountModel? account = default)
    {
        var (page, pageSize, sortColumn, orderByDesc, studentClassIds, startDate, endDate, paymentStatuses) =
            queryTuitionModel;

        var startDateTimeUtc = startDate.HasValue
            ? DateTime.SpecifyKind(startDate.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc)
            : (DateTime?)null;

        var endDateTimeUtc = endDate.HasValue
            ? DateTime.SpecifyKind(endDate.Value.ToDateTime(TimeOnly.MaxValue), DateTimeKind.Utc)
            : (DateTime?)null;

        var test = await _unitOfWork.TuitionRepository.GetAllAsync();

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

        var currentYear = DateTime.UtcNow.Year;

        var currentTaxRateConfig = await _serviceFactory.SystemConfigService.GetTaxesRateConfig(currentYear);

        var currentTaxRate = double.TryParse(currentTaxRateConfig?.ConfigValue, out var taxRate) ? taxRate : 0;

        foreach (var tuitionWithStudentClassModel in result.Items)
        {
            var currentTaxAmount = tuitionWithStudentClassModel.Amount * (decimal)currentTaxRate;
            tuitionWithStudentClassModel.Fee = (double)currentTaxAmount;
        }

        return result;
    }


    public async Task<TuitionWithStudentClassModel> GetTuitionById(Guid tuitionId,
        AccountModel? currentAccount)
    {
        var result = await _unitOfWork.TuitionRepository
            .FindSingleProjectedAsync<TuitionWithStudentClassModel>(e => e.Id == tuitionId, false);

        if (result is null || result.Id == null) throw new NotFoundException("Tuition not found.");

        if (result.StudentClass.StudentFirebaseId != currentAccount!.AccountFirebaseId &&
            currentAccount.Role != Role.Staff)
            throw new ForbiddenMethodException("You are not allowed to see this tuition.");

        return result;
    }

    public async Task CreateTuitionWhenRegisterClass(ClassDetailModel classDetailModel)
    {
        var utcNow = DateTime.UtcNow.AddHours(7);

        var trialSlotConfig = await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.NumTrialSlot);
        int.TryParse(trialSlotConfig.ConfigValue, out var trialSlotCount);

        var deadlinePayTuition =
            await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.TuitionPaymentDeadline);
        double.TryParse(deadlinePayTuition.ConfigValue, out var numOfDeadlineDays);

        var sortedSlots = classDetailModel.Slots.OrderBy(s => s.Date).ToList();
        var lastSlot = sortedSlots.LastOrDefault();

        var tuitions = new List<Tuition>();

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            foreach (var studentClass in classDetailModel.StudentClasses)
            {
                var hasUsedTrial = studentClass.Student.HasUsedTrial;

                var paidSlots = hasUsedTrial
                    ? sortedSlots
                    : sortedSlots.Skip(trialSlotCount);

                var paidSlotCount = paidSlots.Count();

                if (classDetailModel.Level != null && paidSlotCount > 0)
                {
                    // Tính deadline dựa trên slot thử cuối cùng nếu chưa dùng
                    DateTime deadline;
                    if (!hasUsedTrial && trialSlotCount > 0 && trialSlotCount <= sortedSlots.Count)
                    {
                        var lastTrialSlot = sortedSlots[trialSlotCount - 1]; // Lấy slot thử cuối cùng
                        var trialEndDate = lastTrialSlot.Date.ToDateTime(new TimeOnly(0, 0), DateTimeKind.Utc);
                        deadline = trialEndDate.AddDays(numOfDeadlineDays).AddHours(7);
                    }
                    else
                    {
                        deadline = utcNow.AddDays(numOfDeadlineDays);
                    }

                    var tuition = new Tuition
                    {
                        Id = Guid.NewGuid(),
                        StudentClassId = studentClass.Id,
                        StartDate = utcNow,
                        EndDate = lastSlot?.Date.ToDateTime(new TimeOnly(23, 59, 59), DateTimeKind.Utc) ?? utcNow,
                        CreatedAt = utcNow,
                        Amount = classDetailModel.Level.PricePerSlot * paidSlotCount,
                        PaymentStatus = PaymentStatus.Pending,
                        Deadline = deadline
                    };

                    tuitions.Add(tuition);
                }

                // Đánh dấu đã dùng học thử
                if (!hasUsedTrial)
                {
                    var account = await _unitOfWork.AccountRepository.FindSingleAsync(sc =>
                        sc.AccountFirebaseId == studentClass.StudentFirebaseId);
                    account.HasUsedTrial = true;
                    await _unitOfWork.AccountRepository.UpdateAsync(account);
                }
            }

            await _unitOfWork.TuitionRepository.AddRangeAsync(tuitions);
        });


        // Gửi email và notification
        foreach (var result in tuitions)
        {
            var student = classDetailModel.StudentClasses.FirstOrDefault(sc => sc.Id == result.StudentClassId);
            if (student != null)
            {
                var emailParam = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "studentName", student.Student.FullName ?? student.Student.UserName },
                    { "className", classDetailModel.Name },
                    { "amount", result.Amount.ToString("N0") },
                    { "endDate", result.EndDate.ToString("dd/MM/yyyy") },
                    { "startDate", result.StartDate.ToString("dd/MM/yyyy") },
                    { "deadline", result.Deadline.ToString("dd/MM/yyyy") }
                };

                await _serviceFactory.EmailService.SendAsync("NotifyTuitionCreated",
                    [student.Student.Email],
                    null,
                    emailParam);

                await _serviceFactory.NotificationService.SendNotificationAsync(student.StudentFirebaseId,
                    $"The tuition fee for class {classDetailModel.Name} has been created. The deadline is {result.Deadline:dd/MM/yyyy}",
                    "Please pay the tuition fee to continue studying");
            }
        }
    }


    private void ValidateTransaction(Transaction transaction)
    {
        if (transaction.TutionId is null)
            throw new NotFoundException("The TuitionId of this transaction must not be null.");

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

    // public async Task CreateTuitionForTestPurpose()
    // {
    //     // Get a class ID (maybe from configuration or the first active class)
    //     var activeClass = await _unitOfWork.ClassRepository
    //         .FindSingleProjectedAsync<ClassDetailModel>(c => c.Status == ClassStatus.Ongoing && c.IsPublic == true
    //             && c.Id == Guid.Parse("f838e840-e4e5-48ae-aa09-fcb36638a698")
    //         );
    //
    //     if (activeClass != null) await CreateTuitionWhenRegisterClass(activeClass);
    // }
}