using Mapster;
using Microsoft.EntityFrameworkCore;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestResult;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;
using PhotonPiano.BusinessLogic.BusinessModel.Payment;
using PhotonPiano.BusinessLogic.BusinessModel.Query;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;
using PhotonPiano.Shared.Exceptions;


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


    public async Task<PagedResult<EntranceTestDetailModel>> GetPagedEntranceTest(QueryEntranceTestModel query,
        AccountModel currentAccount)
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
                    e => currentAccount.Role != Role.Student || e.EntranceTestStudents.Any(ets =>
                        ets.StudentFirebaseId == currentAccount.AccountFirebaseId),
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

    public async Task<EntranceTestDetailModel> GetEntranceTestDetailById(Guid id, AccountModel currentAccount)
    {
        // var cache = await _serviceFactory.RedisCacheService.GetAsync<EntranceTestDetailModel>($"entranceTest_{id}");
        //
        // if (cache is not null)
        // {
        //     return cache;
        // }

        var entranceTest = await _unitOfWork.EntranceTestRepository
            .FindSingleProjectedAsync<EntranceTestDetailModel>(e => e.Id == id, false,
                option: TrackingOption.IdentityResolution);

        if (entranceTest is null)
        {
            throw new NotFoundException("EntranceTest not found.");
        }

        if (currentAccount.Role == Role.Student &&
            entranceTest.EntranceTestStudents.All(ets => ets.StudentFirebaseId != currentAccount.AccountFirebaseId))
        {
            throw new ForbiddenMethodException("You are not allowed to view this entrance test information.");
        }

        return entranceTest;
    }

    public async Task<EntranceTestDetailModel> CreateEntranceTest(CreateEntranceTestModel entranceTestStudent,
        AccountModel currentAccount)
    {
        var entranceTestModel = entranceTestStudent.Adapt<EntranceTest>();
        entranceTestModel.CreatedById = currentAccount.AccountFirebaseId;

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

        return await GetEntranceTestDetailById(createdEntranceTest.Id, currentAccount);
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

        if (entranceTestEntity is null)
        {
            throw new NotFoundException("This EntranceTest not found.");
        }

        entranceTestStudentModel.Adapt(entranceTestEntity);

        if (entranceTestStudentModel.RoomId.HasValue)
        {
            var room = await _unitOfWork.RoomRepository.FindSingleAsync(
                r => r.Id == entranceTestStudentModel.RoomId.Value,
                hasTrackings: false);

            if (room is null)
            {
                throw new NotFoundException("Room not found.");
            }

            entranceTestEntity.RoomName = room.Name;
        }

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
        {
            // Invalidate specific cache for the entrance test
            await _serviceFactory.RedisCacheService.DeleteAsync($"entranceTest_{id.Value}");
        }
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
            CreatedByEmail = currentAccount.Email,
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
                callbackModel.VnpResponseCode == "00" ? PaymentStatus.Succeed : PaymentStatus.Failed;
            transaction.TransactionCode = callbackModel.VnpTransactionNo;
            transaction.UpdatedAt = DateTime.UtcNow;

            switch (transaction.PaymentStatus)
            {
                case PaymentStatus.Succeed:
                    
                    var entranceTestStudent = new EntranceTestStudent
                    {
                        Id = Guid.CreateVersion7(),
                        StudentFirebaseId = accountId,
                        CreatedAt = DateTime.UtcNow,
                        CreatedById = accountId,
                    };
                    
                    await _unitOfWork.EntranceTestStudentRepository.AddAsync(entranceTestStudent);

                    transaction.EntranceTestStudentId = entranceTestStudent.Id;

                    account.StudentStatus = StudentStatus.WaitingForEntranceTestArrangement;
                    account.UpdatedAt = DateTime.UtcNow;

                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    await _serviceFactory.NotificationService.SendNotificationsToAllStaffsAsync(
                        $"Học viên {account.FullName} vừa đăng ký thi đầu vào", "");

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
                            FullName = student.FullName,
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


    public async Task<List<EntranceTestDetailModel>> AutoArrangeEntranceTests(AutoArrangeEntranceTestsModel model,
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

        if (students.Any(s => s.StudentStatus != StudentStatus.WaitingForEntranceTestArrangement))
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

        entranceTests =
            await _serviceFactory.SchedulerService.AssignInstructorsToEntranceTests(entranceTests, validSlots);

        var arrangedStudentIds = students.Select(s => s.AccountFirebaseId);

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.EntranceTestRepository.AddRangeAsync(entranceTests);

            await _unitOfWork.AccountRepository.ExecuteUpdateAsync(
                expression: a => arrangedStudentIds.Contains(a.AccountFirebaseId),
                setter => setter.SetProperty(a => a.StudentStatus, StudentStatus.AttemptingEntranceTest));
        });

        return entranceTests.Adapt<List<EntranceTestDetailModel>>();
    }

    private static Level GetPianoSkillLevel(decimal bandScore) =>
        bandScore switch
        {
            >= 0 and < 2.5m => Level.Beginner,
            >= 2.5m and < 5.0m => Level.Novice,
            >= 5.0m and < 7.5m => Level.Intermediate,
            >= 7.5m and < 9.5m => Level.Advanced,
            >= 9.5m and <= 10.0m => Level.Virtuoso,
            _ => throw new BadRequestException("Band score must be between 0 and 10.")
        };


    public async Task UpdateStudentEntranceResults(Guid id, string studentId,
        UpdateEntranceTestResultsModel updateModel,
        AccountModel currentAccount)
    {
        var entranceTestStudent = await _unitOfWork.EntranceTestStudentRepository.FindFirstAsync(ets =>
            ets.EntranceTestId == id
            && ets.StudentFirebaseId == studentId);

        if (entranceTestStudent is null)
        {
            throw new NotFoundException("Entrance test not found or student not found.");
        }

        if (!string.IsNullOrEmpty(updateModel.InstructorComment) && currentAccount.Role != Role.Instructor)
        {
            throw new ForbiddenMethodException("You cannot update the instructor comment.");
        }

        if (updateModel.TheoraticalScore.HasValue && currentAccount.Role != Role.Staff)
        {
            throw new ForbiddenMethodException("You cannot update the theoratical score.");
        }

        if (updateModel.UpdateScoreRequests.Count > 0 && currentAccount.Role != Role.Instructor)
        {
            throw new ForbiddenMethodException("You cannot update the practical score results.");
        }

        var entranceTest =
            await _unitOfWork.EntranceTestRepository.FindSingleAsync(et => et.Id == id,
                hasTrackings: false);

        if (currentAccount.Role == Role.Instructor && entranceTest!.InstructorId != currentAccount.AccountFirebaseId)
        {
            throw new ForbiddenMethodException("You cannot update the results.");
        }

        List<EntranceTestResult> results = [];
        decimal bandScore = 0;

        if (updateModel.UpdateScoreRequests.Count > 0)
        {
            var criteriaIds = updateModel.UpdateScoreRequests.Select(s => s.CriteriaId);

            var criterias =
                await _unitOfWork.CriteriaRepository.FindAsync(c => criteriaIds.Contains(c.Id), hasTrackings: false);

            if (criterias.Count != criteriaIds.Count())
            {
                throw new BadRequestException("Some criterias are not found.");
            }

            if ((await _unitOfWork.CriteriaRepository.CountAsync(c => c.For == CriteriaFor.EntranceTest,
                    hasTrackings: false)) != criterias.Count)
            {
                throw new BadRequestException("Not enough criterias.");
            }

            foreach (var score in updateModel.UpdateScoreRequests)
            {
                var criteria = criterias.FirstOrDefault(c => c.Id == score.CriteriaId);
                results.Add(new EntranceTestResult
                {
                    Id = Guid.CreateVersion7(),
                    EntranceTestStudentId = entranceTestStudent.Id,
                    CriteriaId = score.CriteriaId,
                    CriteriaName = criteria?.Name,
                    CreatedById = currentAccount.AccountFirebaseId,
                    Score = score.Score,
                    Weight = criteria?.Weight
                });
            }

            decimal practicalScore = results.Aggregate(decimal.Zero,
                (current, result) => current + result.Score!.Value * result.Weight!.Value);
            
            decimal theoryScore = entranceTestStudent.TheoraticalScore.HasValue 
                ? Convert.ToDecimal(entranceTestStudent.TheoraticalScore.Value) 
                : decimal.Zero;

            bandScore = (theoryScore + practicalScore) / 2;
        }


        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            updateModel.Adapt(entranceTestStudent);

            if (updateModel.UpdateScoreRequests.Count > 0)
            {
                entranceTestStudent.BandScore = bandScore;
                entranceTestStudent.Level = GetPianoSkillLevel(bandScore);
                await _unitOfWork.EntranceTestResultRepository.ExecuteDeleteAsync(etr =>
                    etr.EntranceTestStudentId == entranceTestStudent.Id);
                await _unitOfWork.EntranceTestResultRepository.AddRangeAsync(results);
            }

            if (updateModel.TheoraticalScore.HasValue)
            {
                var dbResults = await _unitOfWork.EntranceTestResultRepository.FindAsync(etr => etr.EntranceTestStudentId == entranceTestStudent.Id,
                    hasTrackings: false);
                
                decimal practicalScore = dbResults.Aggregate(decimal.Zero,
                    (current, result) => current + result.Score!.Value * result.Weight!.Value);
                
                decimal theoryScore = updateModel.TheoraticalScore.HasValue 
                    ? Convert.ToDecimal(updateModel.TheoraticalScore.Value) 
                    : decimal.Zero;


                bandScore = (theoryScore + practicalScore) / 2;
                
                entranceTestStudent.BandScore = bandScore;
                entranceTestStudent.Level = GetPianoSkillLevel(bandScore);
            }

            entranceTestStudent.UpdateById = currentAccount.AccountFirebaseId;
            entranceTestStudent.UpdatedAt = DateTime.UtcNow.AddHours(7);
        });

        // await _serviceFactory.NotificationService.SendNotificationAsync(studentId,
        //     "Điểm thi đầu vào của bạn vừa được cập nhật", "");
    }
}