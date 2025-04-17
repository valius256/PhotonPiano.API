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
using PhotonPiano.Shared.Enums;
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
                    e => currentAccount.Role == Role.Staff || currentAccount.Role == Role.Instructor &&
                         e.InstructorId == currentAccount.AccountFirebaseId ||
                         e.EntranceTestStudents.Any(ets =>
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

        if (entranceTestStudentModel.IsAnnouncedScore.HasValue)
        {
            bool isFullScoreUpdated = true;
            var entranceTestStudents = await _unitOfWork.EntranceTestStudentRepository
                .FindProjectedAsync<EntranceTestStudentWithResultsModel>(
                    ets => ets.EntranceTestId == id,
                    hasTrackings: false);

            foreach (var entranceTestStudent in entranceTestStudents)
            {
                if (entranceTestStudent.EntranceTestResults.Count == 0 ||
                    !entranceTestStudent.TheoraticalScore.HasValue)
                {
                    isFullScoreUpdated = false;
                }
            }

            if (!isFullScoreUpdated)
            {
                throw new BadRequestException("Can't publish the score of this entrance test");
            }

            if (entranceTestStudentModel.IsAnnouncedScore.Value)
            {
                var studentIds = entranceTestStudents.Select(x => x.StudentFirebaseId);

                await _unitOfWork.AccountRepository.ExecuteUpdateAsync(a => studentIds.Contains(a.AccountFirebaseId),
                    setter => setter.SetProperty(x => x.StudentStatus, StudentStatus.WaitingForClass));

                await _serviceFactory.NotificationService.SendNotificationToManyAsync(studentIds.ToList(),
                    "Điểm thi đầu vào của bạn đã được công bố!", "");
            }
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

        if (entranceTestStudent is null)
        {
            throw new NotFoundException("Entrance test student not found or results has not been published.");
        }

        if (entranceTestStudent.EntranceTest?.IsAnnouncedScore == false)
        {
            entranceTestStudent.EntranceTestResults = [];
        }

        if (currentAccount.Role == Role.Student &&
            entranceTestStudent.StudentFirebaseId != currentAccount.AccountFirebaseId)
            throw new ForbiddenMethodException("You are not allowed to access this resource.");

        return entranceTestStudent;
    }

    public async Task RemoveStudentFromTest(Guid testId, string studentId, AccountModel currentAccount)
    {
        var entranceTestStudent =
            await _unitOfWork.EntranceTestStudentRepository.FindFirstAsync(
                ets => ets.EntranceTestId == testId && ets.StudentFirebaseId == studentId);

        if (entranceTestStudent is null)
        {
            throw new NotFoundException("Entrance test student not found or results has not been published.");
        }

        var entranceTest =
            await _unitOfWork.EntranceTestRepository.FindSingleProjectedAsync<EntranceTestWithStudentsModel>(
                e => e.Id == testId,
                hasTrackings: false);

        if (entranceTest?.EntranceTestStudents.Count <= 1)
        {
            throw new BadRequestException("Only 1 student is in the entrance test.");
        }

        entranceTestStudent.RecordStatus = RecordStatus.IsDeleted;
        entranceTestStudent.DeletedAt = DateTime.UtcNow.AddHours(7);
        entranceTestStudent.DeletedById = currentAccount.AccountFirebaseId;

        await _unitOfWork.SaveChangesAsync();
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
        var entranceTestConfigs = await _serviceFactory.SystemConfigService.GetEntranceTestConfigs();

        var allowRegisterConfig =
            entranceTestConfigs.FirstOrDefault(c => c.ConfigName == ConfigNames.AllowEntranceTestRegistering);

        if (allowRegisterConfig is not null)
        {
            bool allowRegistering = Convert.ToBoolean(allowRegisterConfig.ConfigValue);

            if (!allowRegistering)
            {
                throw new BadRequestException("Entrance test registering is closed for now.");
            }
        }

        if (currentAccount.StudentStatus != StudentStatus.DropOut &&
            currentAccount.StudentStatus != StudentStatus.Unregistered)
        {
            throw new BadRequestException(
                "Student is must be in DropOut or Unregistered in order to be accepted to enroll in entrance test.");
        }

        var feeConfig =
            await _unitOfWork.SystemConfigRepository.FindSingleAsync(s => s.ConfigName == ConfigNames.TestFee);

        var transactionId = Guid.NewGuid();

        var transaction = new Transaction
        {
            Id = transactionId,
            TransactionCode = _serviceFactory.TransactionService.GetTransactionCode(TransactionType.EntranceTestFee,
                DateTime.UtcNow.AddHours(7), transactionId),
            Amount = feeConfig != null && !string.IsNullOrEmpty(feeConfig.ConfigValue)
                ? Convert.ToInt32(feeConfig.ConfigValue)
                : 100_000,
            CreatedAt = DateTime.UtcNow.AddHours(7),
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

        transaction.PaymentStatus =
            callbackModel.VnpResponseCode == "00" ? PaymentStatus.Succeed : PaymentStatus.Failed;
        transaction.TransactionCode = callbackModel.VnpTransactionNo;
        transaction.UpdatedAt = DateTime.UtcNow.AddHours(7);

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

                await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    account.StudentStatus = StudentStatus.WaitingForEntranceTestArrangement;
                    account.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.SaveChangesAsync();
                    await _serviceFactory.NotificationService.SendNotificationsToAllStaffsAsync(
                        $"Học viên {account.FullName} vừa đăng ký thi đầu vào", "");
                });


                break;
            case PaymentStatus.Failed:
                throw new BadRequestException("Payment has failed.");
            default:
                throw new BadRequestException("Unknown payment status.");
        }
    }

    private async Task<List<EntranceTest>> CreateAndAssignStudentsToEntranceTests(
        List<AccountDetailModel> students,
        DateTime startDate, DateTime endDate,
        string staffAccountId, params List<Shift> shiftOptions)
    {
        var maxStudentsPerSlotConfig =
            await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.MaxStudentsInTest);
        int maxStudentsPerSlot = Convert.ToInt32(maxStudentsPerSlotConfig.ConfigValue);

        var entranceTests = new List<EntranceTest>();
        var remainingStudents = new List<AccountDetailModel>(students);
        var addingEntranceTestStudents = new List<EntranceTestStudent>();

        var existingTests = await _unitOfWork.EntranceTestRepository.FindProjectedAsync<EntranceTestWithStudentsModel>(
            e => e.Date >= DateOnly.FromDateTime(startDate) && e.Date <= DateOnly.FromDateTime(endDate),
            hasTrackings: false
        );

        var assignedStudents = new HashSet<string>(); // Track globally to prevent multiple assignments

        await foreach (var room in _unitOfWork.RoomRepository
                           .FindAsQueryable(r => r.Status == RoomStatus.Opened, hasTrackings: false)
                           .AsAsyncEnumerable())
        {
            while (remainingStudents.Count > 0)
            {
                //Step 1: Try to assign students to existing entrance tests
                foreach (var entranceTest in existingTests)
                {
                    if (entranceTest.RoomId == room.Id && entranceTest.EntranceTestStudents.Count < maxStudentsPerSlot)
                    {
                        foreach (var student in remainingStudents.ToList()) // Ensure modifications to list
                        {
                            if (entranceTest.EntranceTestStudents.Count >= maxStudentsPerSlot)
                                break; // Test is full

                            if (assignedStudents.Contains(student.AccountFirebaseId))
                                continue; // Prevent multiple assignments

                            var newEntranceTestStudent = new EntranceTestStudent
                            {
                                Id = Guid.NewGuid(),
                                StudentFirebaseId = student.AccountFirebaseId,
                                EntranceTestId = entranceTest.Id,
                                CreatedById = staffAccountId,
                                FullName = student.FullName,
                                CreatedAt = DateTime.UtcNow,
                            };

                            entranceTest.EntranceTestStudents.Add(
                                newEntranceTestStudent.Adapt<EntranceTestStudentModel>());
                            addingEntranceTestStudents.Add(newEntranceTestStudent);
                            assignedStudents.Add(student.AccountFirebaseId);
                        }
                    }
                }

                //Remove assigned students from the remaining list
                remainingStudents.RemoveAll(s => assignedStudents.Contains(s.AccountFirebaseId));

                //Step 2: If there are still unassigned students, create a new entrance test
                if (remainingStudents.Count == 0) break;

                var entranceTestId = Guid.NewGuid();
                var studentsToAssign = remainingStudents
                    .Take(Math.Min(room.Capacity ?? 10, maxStudentsPerSlot))
                    .ToList();

                foreach (var student in studentsToAssign)
                    assignedStudents.Add(student.AccountFirebaseId); // Prevent future assignments

                var newEntranceTest = new EntranceTest
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
                    EntranceTestStudents = studentsToAssign.Select(student => new EntranceTestStudent
                    {
                        Id = Guid.NewGuid(),
                        StudentFirebaseId = student.AccountFirebaseId,
                        CreatedById = staffAccountId,
                        FullName = student.FullName,
                        CreatedAt = DateTime.UtcNow,
                    }).ToList()
                };

                entranceTests.Add(newEntranceTest);
                remainingStudents.RemoveAll(s => assignedStudents.Contains(s.AccountFirebaseId));
            }
        }

        if (addingEntranceTestStudents.Count > 0)
        {
            await _unitOfWork.EntranceTestStudentRepository.AddRangeAsync(addingEntranceTestStudents);
            await _unitOfWork.SaveChangesAsync();
        }

        return entranceTests;
    }

    private string GetEntranceTestName(EntranceTest entranceTest)
    {
        string shiftName = entranceTest.Shift switch
        {
            Shift.Shift1_7h_8h30 => "7h_8h30",
            Shift.Shift2_8h45_10h15 => "8h45_10h15",
            Shift.Shift3_10h45_12h => "10h45_12h",
            Shift.Shift4_12h30_14h00 => "12h30_14h00",
            Shift.Shift5_14h15_15h45 => "14h15_15h45",
            Shift.Shift6_16h00_17h30 => "16h00_17h30",
            Shift.Shift7_18h_19h30 => "18h_19h30",
            Shift.Shift8_19h45_21h15 => "19h45_21h15",
            _ => entranceTest.Shift.ToString()
        };

        return $"{shiftName}_{entranceTest.Date:yyyy-MM-dd}_{entranceTest.RoomName}";
    }

    public async Task AutoArrangeEntranceTests(AutoArrangeEntranceTestsModel model,
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

        var existingEntranceTests = await _unitOfWork.EntranceTestRepository
            .FindAsync(e => e.Date >= DateOnly.FromDateTime(startDate) && e.Date <= DateOnly.FromDateTime(endDate),
                hasTrackings: false);

        var entranceTests = await CreateAndAssignStudentsToEntranceTests(students,
            startDate, endDate,
            currentAccount.AccountFirebaseId, shiftOptions);

        if (entranceTests.Count == 0)
        {
            return;
        }

        var conflictGraph = _serviceFactory.SchedulerService.BuildEntranceTestsConflictGraph(entranceTests);

        var dayOffs = await _unitOfWork.DayOffRepository.GetAllAsync(hasTrackings: false);

        List<DateTime> holidays = [];

        foreach (var dayOff in dayOffs)
        {
            holidays.AddRange(Enumerable.Range(0, (dayOff.EndTime - dayOff.StartTime).Days + 1)
                .Select(d => dayOff.StartTime.AddDays(d)));
        }

        var validSlots =
            await _serviceFactory.SchedulerService.GenerateValidTimeSlots(existingEntranceTests, startDate, endDate,
                holidays, shiftOptions);

        entranceTests = await _serviceFactory.SchedulerService.AssignTimeSlotsToEntranceTests(entranceTests,
            conflictGraph,
            startDate, endDate,
            validSlots, existingEntranceTests);

        entranceTests =
            await _serviceFactory.SchedulerService.AssignInstructorsToEntranceTests(entranceTests, validSlots);

        var arrangedStudentIds = students.Select(s => s.AccountFirebaseId);

        foreach (var test in entranceTests)
        {
            test.Name = GetEntranceTestName(test);
        }

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.EntranceTestRepository.AddRangeAsync(entranceTests);

            await _unitOfWork.AccountRepository.ExecuteUpdateAsync(
                expression: a => arrangedStudentIds.Contains(a.AccountFirebaseId),
                setter => setter.SetProperty(a => a.StudentStatus, StudentStatus.AttemptingEntranceTest));
        });
    }

    private static TimeOnly GetShiftStartTime(Shift shift)
    {
        return shift switch
        {
            Shift.Shift1_7h_8h30 => new TimeOnly(7, 0),
            Shift.Shift2_8h45_10h15 => new TimeOnly(8, 45),
            Shift.Shift3_10h45_12h => new TimeOnly(10, 45),
            Shift.Shift4_12h30_14h00 => new TimeOnly(12, 30),
            Shift.Shift5_14h15_15h45 => new TimeOnly(14, 15),
            Shift.Shift6_16h00_17h30 => new TimeOnly(16, 0),
            Shift.Shift7_18h_19h30 => new TimeOnly(18, 0),
            Shift.Shift8_19h45_21h15 => new TimeOnly(19, 45),
            _ => throw new ArgumentOutOfRangeException(nameof(shift), shift, null)
        };
    }

    private static TimeOnly GetShiftEndTime(Shift shift)
    {
        return shift switch
        {
            Shift.Shift1_7h_8h30 => new TimeOnly(8, 30),
            Shift.Shift2_8h45_10h15 => new TimeOnly(10, 15),
            Shift.Shift3_10h45_12h => new TimeOnly(12, 0),
            Shift.Shift4_12h30_14h00 => new TimeOnly(14, 0),
            Shift.Shift5_14h15_15h45 => new TimeOnly(15, 45),
            Shift.Shift6_16h00_17h30 => new TimeOnly(17, 30),
            Shift.Shift7_18h_19h30 => new TimeOnly(19, 30),
            Shift.Shift8_19h45_21h15 => new TimeOnly(21, 15),
            _ => throw new ArgumentOutOfRangeException(nameof(shift), shift, null)
        };
    }

    private static bool HasShiftEnded(DateOnly dateToCompare, Shift shift)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));

        if (today > dateToCompare)
        {
            return true;
        }

        if (today == dateToCompare)
        {
            var shiftEndTime = GetShiftEndTime(shift);
            var shiftEndDateTime = dateToCompare.ToDateTime(shiftEndTime);

            return DateTime.Now > shiftEndDateTime;
        }

        return false;
    }

    public static EntranceTestStatus GetEntranceTestStatus(DateOnly testDate, Shift shift)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));
        var now = DateTime.UtcNow.AddHours(7);

        if (today < testDate)
        {
            return EntranceTestStatus.NotStarted;
        }

        if (today > testDate)
        {
            return EntranceTestStatus.Ended;
        }

        var startTime = GetShiftStartTime(shift);
        var endTime = GetShiftEndTime(shift);

        var shiftStartDateTime = testDate.ToDateTime(startTime);
        var shiftEndDateTime = testDate.ToDateTime(endTime);

        if (now < shiftStartDateTime)
        {
            return EntranceTestStatus.NotStarted;
        }

        if (now >= shiftStartDateTime && now <= shiftEndDateTime)
        {
            return EntranceTestStatus.OnGoing;
        }

        return EntranceTestStatus.Ended;
    }


    public async Task UpdateStudentsEntranceTestResults(UpdateStudentsEntranceTestResultsModel updateModel,
        Guid entranceTestId,
        AccountModel currentAccount)
    {
        var entranceTest = await _unitOfWork.EntranceTestRepository.FindSingleAsync(e => e.Id == entranceTestId);

        if (entranceTest is null)
        {
            throw new NotFoundException("Entrance test not found.");
        }

        if (!HasShiftEnded(entranceTest.Date, entranceTest.Shift))
        {
            throw new BadRequestException("Entrance test has not ended.");
        }

        var entranceTestStudents =
            await _unitOfWork.EntranceTestStudentRepository.GetEntranceTestStudentsWithResults(entranceTestId);

        if (entranceTestStudents.Count != updateModel.UpdateRequests.Count)
        {
            throw new BadRequestException("Some students are not valid to be updated.");
        }

        var criterias =
            await _unitOfWork.CriteriaRepository.FindAsync(c => c.For == CriteriaFor.EntranceTest, hasTrackings: false);

        List<EntranceTestResult> entranceTestResultsToAdd = [];

        foreach (var entranceTestStudent in entranceTestStudents)
        {
            var requestModel =
                updateModel.UpdateRequests.FirstOrDefault(x => x.StudentId == entranceTestStudent.StudentFirebaseId);

            if (requestModel is null)
            {
                continue;
            }

            var theoryScore = requestModel.TheoraticalScore;
            decimal practicalScore = 0;

            if (currentAccount.Role == Role.Staff)
            {
                entranceTestStudent.TheoraticalScore = theoryScore;
            }

            if (currentAccount.Role == Role.Instructor)
            {
                entranceTestStudent.InstructorComment = requestModel.InstructorComment;
            }

            entranceTestStudent.EntranceTestResults.Clear();

            foreach (var result in requestModel.Scores)
            {
                var criteria = criterias.FirstOrDefault(c => c.Id == result.CriteriaId);

                if (criteria is null)
                {
                    throw new NotFoundException($"Criteria with id {result.CriteriaId} not found.");
                }

                var resultToAdd = new EntranceTestResult
                {
                    Id = Guid.NewGuid(),
                    EntranceTestStudentId = entranceTestStudent.Id,
                    CreatedById = currentAccount.AccountFirebaseId,
                    CriteriaId = result.CriteriaId,
                    CriteriaName = criteria.Name,
                    Score = result.Score,
                    Weight = criteria.Weight
                };

                entranceTestResultsToAdd.Add(resultToAdd);

                entranceTestStudent.EntranceTestResults.Add(resultToAdd);

                practicalScore += result.Score * (criteria.Weight / 100);

                entranceTestStudent.LevelId = await _serviceFactory.LevelService.GetLevelIdFromScores(
                    Convert.ToDecimal(entranceTestStudent.TheoraticalScore ?? 0), practicalScore);
            }

            entranceTestStudent.BandScore = ((decimal)theoryScore + practicalScore) / 2;
        }

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.EntranceTestResultRepository.AddRangeAsync(entranceTestResultsToAdd);
        });
    }

    public async Task<(int theoryPercentage, int practicalPercentage)> GetScorePercentagesAsync()
    {
        var configs =
            await _serviceFactory.SystemConfigService.GetEntranceTestConfigs(
                ConfigNames.TheoryPercentage, ConfigNames.PracticePercentage
            );

        var theoryConfig = configs.FirstOrDefault(c => c.ConfigName == ConfigNames.TheoryPercentage);

        var practiceConfig = configs.FirstOrDefault(c => c.ConfigName == ConfigNames.PracticePercentage);

        var theoryPercentage = theoryConfig is not null && !string.IsNullOrEmpty(theoryConfig.ConfigValue)
            ? Convert.ToInt32(theoryConfig.ConfigValue)
            : 50;

        var practicalPercentage = practiceConfig is not null && !string.IsNullOrEmpty(practiceConfig.ConfigValue)
            ? Convert.ToInt32(practiceConfig.ConfigValue)
            : 50;

        return (theoryPercentage, practicalPercentage);
    }

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

        if (updateModel.LevelId.HasValue)
        {
            if (!await _unitOfWork.LevelRepository.AnyAsync(l => l.Id == updateModel.LevelId.Value))
            {
                throw new NotFoundException("Level not found.");
            }
        }

        var entranceTest =
            await _unitOfWork.EntranceTestRepository.FindSingleAsync(et => et.Id == id,
                hasTrackings: false);

        if (entranceTest is null)
        {
            throw new NotFoundException("Entrance test not found.");
        }

        if (currentAccount.Role == Role.Instructor && entranceTest!.InstructorId != currentAccount.AccountFirebaseId)
        {
            throw new ForbiddenMethodException("You cannot update the results.");
        }

        if (!HasShiftEnded(entranceTest.Date, entranceTest.Shift))
        {
            throw new BadRequestException("Entrance test has not ended.");
        }

        var (theoryPercentage, practicalPercentage) = await GetScorePercentagesAsync();

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
                    Id = Guid.NewGuid(),
                    EntranceTestStudentId = entranceTestStudent.Id,
                    CriteriaId = score.CriteriaId,
                    CriteriaName = criteria?.Name,
                    CreatedById = currentAccount.AccountFirebaseId,
                    Score = score.Score,
                    Weight = criteria?.Weight
                });
            }

            decimal practicalScore = results.Aggregate(decimal.Zero,
                (current, result) => current + result.Score!.Value * (result.Weight!.Value / 100));

            decimal theoryScore = entranceTestStudent.TheoraticalScore.HasValue
                ? Convert.ToDecimal(entranceTestStudent.TheoraticalScore.Value)
                : decimal.Zero;

            bandScore = (theoryScore * theoryPercentage / 100 + practicalScore * practicalPercentage / 100);
        }

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var oldLevelId = entranceTestStudent.LevelId;

            updateModel.Adapt(entranceTestStudent);

            var dbResults = await _unitOfWork.EntranceTestResultRepository.FindAsync(
                etr => etr.EntranceTestStudentId == entranceTestStudent.Id,
                hasTrackings: false);

            decimal practicalScore = dbResults.Aggregate(decimal.Zero,
                (current, result) => current + result.Score!.Value * (result.Weight!.Value / 100));

            if (updateModel.UpdateScoreRequests.Count > 0 && currentAccount.Role == Role.Instructor)
            {
                entranceTestStudent.InstructorComment = updateModel.InstructorComment;
                entranceTestStudent.BandScore = bandScore;
                entranceTestStudent.LevelId = await _serviceFactory.LevelService.GetLevelIdFromScores(
                    Convert.ToDecimal(entranceTestStudent.TheoraticalScore ?? 0), practicalScore);
                await _unitOfWork.AccountRepository.ExecuteUpdateAsync(a => a.AccountFirebaseId == studentId,
                    setter => setter.SetProperty(s => s.LevelId, entranceTestStudent.LevelId));
                await _unitOfWork.EntranceTestResultRepository.ExecuteDeleteAsync(etr =>
                    etr.EntranceTestStudentId == entranceTestStudent.Id);
                await _unitOfWork.EntranceTestResultRepository.AddRangeAsync(results);
            }

            if (updateModel.TheoraticalScore.HasValue && currentAccount.Role == Role.Staff)
            {
                entranceTestStudent.TheoraticalScore = updateModel.TheoraticalScore;
                decimal theoryScore = updateModel.TheoraticalScore.HasValue
                    ? Convert.ToDecimal(updateModel.TheoraticalScore.Value)
                    : decimal.Zero;

                bandScore = (theoryScore * theoryPercentage / 100 + practicalScore * practicalPercentage / 100);

                if (entranceTestStudent.LevelId.HasValue && updateModel.LevelId.HasValue &&
                    updateModel.LevelId != oldLevelId)
                {
                    entranceTestStudent.LevelId = updateModel.LevelId;
                    entranceTestStudent.LevelAdjustedAt = DateTime.UtcNow.AddHours(7);
                }
                else
                {
                    entranceTestStudent.LevelId = await _serviceFactory.LevelService.GetLevelIdFromScores(
                        theoryScore, practicalScore);
                }

                entranceTestStudent.BandScore = bandScore;
                await _unitOfWork.AccountRepository.ExecuteUpdateAsync(a => a.AccountFirebaseId == studentId,
                    setter => setter.SetProperty(s => s.LevelId, entranceTestStudent.LevelId));
            }

            entranceTestStudent.UpdateById = currentAccount.AccountFirebaseId;
            entranceTestStudent.UpdatedAt = DateTime.UtcNow.AddHours(7);
        });


        // await _serviceFactory.NotificationService.SendNotificationAsync(studentId,
        //     "Điểm thi đầu vào của bạn vừa được cập nhật", "");
    }

    public async Task UpdateEntranceTestsMaxStudents(int maxStudents, AccountModel currentAccount)
    {
        var config =
            await _unitOfWork.SystemConfigRepository.FindSingleAsync(c =>
                c.ConfigName == "Số học viên tối đa của ca thi");

        if (config is null)
        {
            throw new NotFoundException("Config not found.");
        }

        config.ConfigValue = maxStudents.ToString();
        config.UpdatedAt = DateTime.UtcNow.AddHours(7);

        await _unitOfWork.SaveChangesAsync();
    }
}