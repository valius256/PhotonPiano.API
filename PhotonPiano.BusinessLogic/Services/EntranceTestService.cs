using Mapster;
using Microsoft.EntityFrameworkCore;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;
using PhotonPiano.BusinessLogic.BusinessModel.Payment;
using PhotonPiano.BusinessLogic.BusinessModel.Query;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;
using PhotonPiano.Shared.Exceptions;
using PhotonPiano.Shared.Utils;

namespace PhotonPiano.BusinessLogic.Services;

public class EntranceTestService : IEntranceTestService
{
    private readonly IServiceFactory _serviceFactory;
    private readonly IUnitOfWork _unitOfWork;

    public EntranceTestService(IUnitOfWork unitOfWork, IServiceFactory serviceFactory)
    {
        _unitOfWork = unitOfWork;
        _serviceFactory = serviceFactory;
    }


    public async Task<PagedResult<EntranceTestDetailModel>> GetPagedEntranceTest(QueryEntranceTestModel query)
    {
        var (page, pageSize, sortColumn, orderByDesc,
                roomIds, keyword, shifts,
                entranceTestIds, isAnnouncedScore,
                instructorIds)
            = query;

        // vi khong search neu dung cache dc do EF func k xai voi cache dc 
        // var cache = await _serviceFactory.RedisCacheService
        //     .GetAsync<PagedResult<EntranceTestDetailModel>>
        //         ($"entranceTests_page{page}_pageSize{pageSize}");
        // ("entranceTests");

        // if (cache is not null )
        // {
        //     var cacheResult = QueryExtension.ProcessData(
        //         cache.Items,
        //         query.Page,
        //         query.PageSize,
        //         query.SortColumn,
        //         query.OrderByDesc,
        //         e =>
        //             (query.RoomIds == null || query.RoomIds.Count == 0 || query.RoomIds.Contains(e.RoomId)) &&
        //             (query.IsOpen == null || e.IsOpen == query.IsOpen.Value) &&
        //             (query.IsAnnouncedScore == null || e.IsAnnouncedScore == query.IsAnnouncedScore.Value) &&
        //             (query.Shifts == null || query.Shifts.Count == 0 || query.Shifts.Contains(e.Shift)) &&
        //             (query.InstructorIds == null || query.InstructorIds.Count == 0 ||
        //              query.InstructorIds.Contains(e.InstructorId)) &&
        //             (query.EntranceTestIds == null || query.EntranceTestIds.Count == 0 ||
        //              (query.EntranceTestIds.Contains(e.Id) &&
        //               (string.IsNullOrEmpty(keyword) ||
        //                (!string.IsNullOrEmpty(e.RoomName) && e.RoomName.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
        //                (!string.IsNullOrEmpty(e.InstructorName) && 
        //                 e.InstructorName.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
        //              )));
        //
        //     return cacheResult;
        // }

        var likeKeyword = query.GetLikeKeyword();

        var result = await _unitOfWork.EntranceTestRepository
            .GetPaginatedWithProjectionAsync<EntranceTestDetailModel>(
                page, pageSize, sortColumn, orderByDesc,
                expressions:
                [
                    e => roomIds != null && (roomIds.Count == 0 || roomIds.Contains(e.RoomId)),
                    e => isAnnouncedScore == null || e.IsAnnouncedScore == isAnnouncedScore.Value,
                    e => shifts != null && (shifts.Count == 0 || shifts.Contains(e.Shift)),
                    e => instructorIds != null &&
                         (instructorIds.Count == 0 || instructorIds.Contains(e.InstructorId ?? string.Empty)),
                    e => entranceTestIds != null && (entranceTestIds.Count == 0 || entranceTestIds.Contains(e.Id)),

                    // search 
                    e => string.IsNullOrEmpty(keyword) ||
                         EF.Functions.ILike(EF.Functions.Unaccent(e.RoomName ?? string.Empty), likeKeyword) ||
                         EF.Functions.ILike(EF.Functions.Unaccent(e.InstructorName ?? string.Empty), likeKeyword) ||
                         EF.Functions.ILike(EF.Functions.Unaccent(e.Name), likeKeyword)
                ]);

        // await _serviceFactory.RedisCacheService
        //     .SaveAsync($"entranceTests_page{query.Page}_pageSize{query.PageSize}_keyword{query.Keyword}",
        //         result, TimeSpan.FromHours(3));
        return result;
    }

    public async Task<EntranceTestDetailModel> GetEntranceTestDetailById(Guid id)
    {
        var cache =
            await _serviceFactory.RedisCacheService.GetAsync<EntranceTestDetailModel>($"entranceTest_{id}");
        if (cache is not null) return cache;

        var result = await _unitOfWork.EntranceTestRepository
            .FindSingleProjectedAsync<EntranceTestDetailModel>(e => e.Id == id, false);
        if (result is null) throw new NotFoundException("EntranceTest not found.");
        return result;
    }

    public async Task<EntranceTestDetailModel> CreateEntranceTest(CreateEntranceTestModel entranceTestStudent,
        string? currentUserFirebaseId = default)
    {
        var entranceTestModel = entranceTestStudent.Adapt<EntranceTest>();
        entranceTestModel.CreatedById = currentUserFirebaseId!;

        // check room is exist 
        var roomDetailModel = await _serviceFactory.RoomService.GetRoomDetailById(entranceTestModel.RoomId);
        // check Instructor is Exist in db
        var instructorDetailModel =
            await _serviceFactory.AccountService.GetAccountById(entranceTestModel.InstructorId!);
        if (instructorDetailModel.Role != Role.Instructor)
            throw new BadRequestException("This is not Instructor, please try again");

        EntranceTest? createdEntranceTest = null;
        entranceTestModel.RoomName = roomDetailModel.Name;
        entranceTestModel.InstructorName = instructorDetailModel.UserName;
        entranceTestModel.RoomCapacity = roomDetailModel.Capacity;

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var createdEntranceTestEntity = await _unitOfWork.EntranceTestRepository.AddAsync(entranceTestModel);
            createdEntranceTest = createdEntranceTestEntity;
        });

        await InvalidateEntranceTestCache();
        await _serviceFactory.RedisCacheService.SaveAsync($"entranceTest_{createdEntranceTest!.Id}",
            createdEntranceTest.Adapt<EntranceTestDetailModel>(),
            TimeSpan.FromHours(5));

        return await GetEntranceTestDetailById(createdEntranceTest.Id);
    }

    public async Task DeleteEntranceTest(Guid id, string? currentUserFirebaseId = default)
    {
        var entranceTestEntity = await _unitOfWork.EntranceTestRepository.FindSingleAsync(q => q.Id == id);
        if (entranceTestEntity is null) throw new NotFoundException("This EntranceTest not found.");

        entranceTestEntity.DeletedById = currentUserFirebaseId;
        entranceTestEntity.DeletedAt = DateTime.UtcNow.AddHours(7);
        entranceTestEntity.RecordStatus = RecordStatus.IsDeleted;

        await _unitOfWork.SaveChangesAsync();

        await _serviceFactory.RedisCacheService.SaveAsync($"entranceTest_{id}", entranceTestEntity,
            TimeSpan.FromHours(5));
    }

    public async Task UpdateEntranceTest(Guid id, UpdateEntranceTestModel entranceTestStudentModel,
        string? currentUserFirebaseId = default)
    {
        var entranceTestEntity = await _unitOfWork.EntranceTestRepository.FindSingleAsync(q => q.Id == id);

        if (entranceTestEntity is null) throw new NotFoundException("This EntranceTest not found.");

        entranceTestStudentModel.Adapt(entranceTestEntity);

        entranceTestEntity.UpdateById = currentUserFirebaseId;
        entranceTestEntity.UpdatedAt = DateTime.UtcNow.AddHours(7);

        await _unitOfWork.SaveChangesAsync();
        await _serviceFactory.RedisCacheService.SaveAsync($"entranceTest_{id}", entranceTestEntity,
            TimeSpan.FromHours(5));
        await InvalidateEntranceTestCache(id);
    }

    public async Task<PagedResult<EntranceTestStudentDetail>> GetPagedEntranceTestStudent(QueryPagedModel query,
        Guid entranceTestId, AccountModel currentAccount)
    {
        if (!await _unitOfWork.EntranceTestRepository.AnyAsync(et => et.Id == entranceTestId))
            throw new NotFoundException("Entrance test not found.");

        var (page, size, column, desc) = query;

        var pagedResult = await _unitOfWork.EntranceTestStudentRepository
            .GetPaginatedWithProjectionAsync<EntranceTestStudentDetail>(
                page,
                size,
                column,
                desc,
                expressions:
                [
                    ets => ets.EntranceTestId == entranceTestId,
                    ets => currentAccount.Role != Role.Student ||
                           ets.StudentFirebaseId == currentAccount.AccountFirebaseId
                ]
            );

        return pagedResult;
    }

    public async Task<EntranceTestStudentDetail> GetEntranceTestStudentDetail(Guid entranceTestId, string studentId,
        AccountModel currentAccount)
    {
        var entranceTestStudent =
            await _unitOfWork.EntranceTestStudentRepository.FindFirstProjectedAsync<EntranceTestStudentDetail>(
                ets => ets.EntranceTestId == entranceTestId && ets.StudentFirebaseId == studentId);

        if (entranceTestStudent is null) throw new NotFoundException("Entrance test student not found");

        if (currentAccount.Role == Role.Student &&
            entranceTestStudent.StudentFirebaseId != currentAccount.AccountFirebaseId)
            throw new ForbiddenMethodException("You are not allowed to access this resource.");

        return entranceTestStudent;
    }

    private async Task InvalidateEntranceTestCache(Guid? id = null)
    {
        // Invalidate general cache
        await _serviceFactory.RedisCacheService.DeleteAsync("entranceTests");

        if (id.HasValue)
            // Invalidate specific cache for the entrance test
            await _serviceFactory.RedisCacheService.DeleteAsync($"entranceTest_{id.Value}");
    }

    public async Task<string> EnrollEntranceTest(AccountModel currentAccount, string returnUrl, string ipAddress,
        string apiBaseUrl)
    {
        if (currentAccount.StudentStatus != StudentStatus.DropOut &&
            currentAccount.StudentStatus != StudentStatus.Unregistered)
        {
            throw new BadRequestException(
                "Student is must be in DropOut or Unregistered in order to be accepted to enroll in entrance test.");
        }

        var transaction = new Transaction
        {
            Id = Guid.CreateVersion7(),
            Amount = 100_000,
            CreatedAt = DateTime.UtcNow,
            CreatedById = currentAccount.AccountFirebaseId,
            TransactionType = TransactionType.EntranceTestFee,
            PaymentStatus = PaymentStatus.Pending,
            PaymentMethod = PaymentMethod.VnPay,
            CreatedByEmail = currentAccount.Email
        };

        await _unitOfWork.TransactionRepository.AddAsync(transaction);

        await _unitOfWork.SaveChangesAsync();

        return _serviceFactory.PaymentService.CreateVnPayPaymentUrl(transaction, ipAddress, apiBaseUrl,
            currentAccount.AccountFirebaseId,
            returnUrl);
    }

    public async Task HandleEnrollmentPaymentCallback(VnPayCallbackModel callbackModel, string accountId)
    {
        Guid transactionCode = Guid.Parse(callbackModel.VnpTxnRef);

        var transaction = await _unitOfWork.TransactionRepository.FindSingleAsync(t => t.Id == transactionCode);

        if (transaction is null)
        {
            throw new PaymentRequiredException("Payment is required");
        }

        var account = await _unitOfWork.AccountRepository.FindSingleAsync(a => a.AccountFirebaseId == accountId);

        if (account is null)
        {
            throw new NotFoundException("Account not found");
        }

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
                    await _unitOfWork.EntranceTestStudentRepository.AddAsync(new EntranceTestStudent
                    {
                        Id = Guid.CreateVersion7(),
                        StudentFirebaseId = accountId,
                        CreatedAt = DateTime.UtcNow,
                        CreatedById = accountId,
                    });

                    account.StudentStatus = StudentStatus.AttemptingEntranceTest;
                    account.UpdatedAt = DateTime.UtcNow;

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

    private async Task<List<EntranceTest>> CreateAndAssignStudentsToEntranceTests(List<AccountDetailModel> students,
        DateTime startDate, DateTime endDate,
        string staffAccountId, int maxStudentsPerSlot = 10, params List<Shift> shiftOptions)
    {
        var entranceTests = new List<EntranceTest>();
        var remainingStudents = new List<AccountDetailModel>(students); // Track remaining students globally

        await foreach (var room in _unitOfWork.RoomRepository
                           .FindAsQueryable(r => r.Status == RoomStatus.Opened, hasTrackings: false)
                           .AsAsyncEnumerable())
        {
            while (remainingStudents.Count > 0)
            {
                // Create a unique EntranceTest ID
                var entranceTestId = Guid.CreateVersion7();

                // Determine the number of students to assign to this test
                var studentsToAssign =
                    remainingStudents.Take(Math.Min(room.Capacity ?? 10, maxStudentsPerSlot)).ToList();

                // Create the EntranceTest object
                var entranceTest = new EntranceTest
                {
                    Id = entranceTestId,
                    Name = $"THI DAU VAO_{entranceTestId}",
                    RoomId = room.Id,
                    RoomName = room.Name,
                    RoomCapacity = room.Capacity,
                    Shift = shiftOptions.Count == 0 ? Shift.Shift1_7h_8h30 : shiftOptions[0],
                    Date = DateOnly.FromDateTime(startDate),
                    CreatedById = staffAccountId,
                    CreatedAt = DateTime.UtcNow,
                    EntranceTestStudents = studentsToAssign.Select(student =>
                        new EntranceTestStudent
                        {
                            Id = Guid.CreateVersion7(),
                            StudentFirebaseId = student.AccountFirebaseId,
                            CreatedById = staffAccountId,
                            CreatedAt = DateTime.UtcNow,
                        }).ToList()
                };

                // Add the entrance test to the list
                entranceTests.Add(entranceTest);

                // Remove the assigned students from the remaining list
                remainingStudents = remainingStudents.Skip(studentsToAssign.Count).ToList();
            }
        }

        return entranceTests;
    }


    public async Task<List<EntranceTestModel>> AutoArrangeEntranceTests(AutoArrangeEntranceTestsModel model,
        AccountModel currentAccount)
    {
        var (studentIds, startDate, endDate, shiftOptions) = model;

        var students = await _unitOfWork.AccountRepository.FindProjectedAsync<AccountDetailModel>(a =>
                studentIds.Contains(a.AccountFirebaseId)
                && a.Role == Role.Student,
            hasTrackings: false);

        if (students.Count != studentIds.Count)
        {
            throw new BadRequestException("Some students are not found.");
        }

        if (students.Any(s => s.StudentStatus != StudentStatus.AttemptingEntranceTest))
        {
            throw new BadRequestException("Some students are not valid to be arranged with entrance tests.");
        }

        if (students.Any(s => s.EntranceTestStudents.Any(ets => ets.EntranceTestId != null)))
        {
            var arrangedStudents = students.Where(s => s.EntranceTestStudents.Any(ets => ets.EntranceTestId != null));

            throw new ConflictException(
                $"Students: {string.Join(", ", arrangedStudents.Select(s => $"{s.AccountFirebaseId}-{s.Email}"))} are already arranged.");
        }

        var entranceTests = await CreateAndAssignStudentsToEntranceTests(students,
            startDate, endDate,
            currentAccount.AccountFirebaseId, 10, shiftOptions);

        var conflictGraph = _serviceFactory.SchedulerService.BuildEntranceTestsConflictGraph(entranceTests);

        var dayOffs = await _unitOfWork.DayOffRepository.GetAllAsync(hasTrackings: false);

        List<DateTime> holidays = [];

        foreach (var dayOff in dayOffs)
        {
            holidays.AddRange(Enumerable.Range(0, (dayOff.EndTime - dayOff.StartTime).Days + 1)
                .Select(d => dayOff.StartTime.AddDays(d)));
        }

        var validSlots =
            _serviceFactory.SchedulerService.GenerateValidTimeSlots(startDate, endDate, holidays, shiftOptions);

        entranceTests = await _serviceFactory.SchedulerService.AssignTimeSlotsToEntranceTests(entranceTests,
            conflictGraph,
            startDate, endDate,
            validSlots);

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.EntranceTestRepository.AddRangeAsync(entranceTests);
        });

        return entranceTests.Adapt<List<EntranceTestModel>>();
    }
}