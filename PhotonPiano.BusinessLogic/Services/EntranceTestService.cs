using Mapster;
using Microsoft.EntityFrameworkCore;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestResult;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;
using PhotonPiano.BusinessLogic.BusinessModel.Payment;
using PhotonPiano.BusinessLogic.BusinessModel.Query;
using PhotonPiano.BusinessLogic.BusinessModel.Slot;
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

    public async Task<EntranceTestDetailModel> CreateEntranceTest(CreateEntranceTestModel createModel,
        AccountModel currentAccount)
    {
        var configs = await _serviceFactory.SystemConfigService.GetConfigs([
            ConfigNames.MinStudentsInTest, ConfigNames.MaxStudentsInTest
        ]);

        var minConfig = configs.FirstOrDefault(c => c.ConfigName == ConfigNames.MinStudentsInTest);

        var maxConfig = configs.FirstOrDefault(c => c.ConfigName == ConfigNames.MaxStudentsInTest);

        if (minConfig is not null && !string.IsNullOrEmpty(minConfig.ConfigValue))
        {
            int minStudents = Convert.ToInt32(minConfig.ConfigValue);

            if (createModel.StudentIds.Count < minStudents)
            {
                throw new BadRequestException($"Test must have at least {minStudents} learners");
            }
        }

        if (maxConfig is not null && !string.IsNullOrEmpty(maxConfig.ConfigValue))
        {
            int maxStudents = Convert.ToInt32(maxConfig.ConfigValue);

            if (createModel.StudentIds.Count > maxStudents)
            {
                throw new BadRequestException($"Test can only have maximum of {maxStudents} learners");
            }
        }

        var entranceTestId = Guid.NewGuid();
        var entranceTestStudents = new List<EntranceTestStudent>();

        if (createModel.StudentIds.Count > 0)
        {
            var students =
                await _unitOfWork.AccountRepository.GetStudentsWithEntranceTestStudents(
                    StudentStatus.WaitingForEntranceTestArrangement,
                    createModel.StudentIds);

            if (students.Count != createModel.StudentIds.Count)
            {
                throw new BadRequestException("Some students are invalid.");
            }

            foreach (var student in students)
            {
                var entranceTestStudent = student.EntranceTestStudents.FirstOrDefault(ets =>
                    ets.StudentFirebaseId == currentAccount.AccountFirebaseId
                    && ets.EntranceTestId == null);

                if (entranceTestStudent is not null)
                {
                    entranceTestStudent.EntranceTestId = entranceTestId;
                }
            }

            entranceTestStudents = createModel.StudentIds.Select(studentId =>
                new EntranceTestStudent
                {
                    Id = Guid.NewGuid(),
                    StudentFirebaseId = studentId,
                    EntranceTestId = entranceTestId,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    CreatedById = currentAccount.AccountFirebaseId,
                }).ToList();
        }

        var entranceTest = createModel.Adapt<EntranceTest>();
        entranceTest.Id = entranceTestId;
        entranceTest.CreatedById = currentAccount.AccountFirebaseId;

        // check room is exist 
        var roomDetailModel = await _serviceFactory.RoomService.GetRoomDetailById(entranceTest.RoomId);

        // check Instructor is Exist in db
        var instructor =
            await _serviceFactory.AccountService.GetAccountById(entranceTest.InstructorId!);

        if (instructor.Role != Role.Instructor)
        {
            throw new BadRequestException("This is not Instructor, please try again");
        }

        if (await _unitOfWork.EntranceTestRepository.AnyAsync(t => t.Date == createModel.Date
                                                                   && t.RoomId == entranceTest.RoomId
                                                                   && t.Shift == entranceTest.Shift))
        {
            throw new ConflictException("There is already an entrance test with the same date, shift and room.");
        }

        var dayOffs = await _unitOfWork.DayOffRepository.GetAllAsync(hasTrackings: false);

        foreach (var dayOff in dayOffs)
        {
            if (createModel.Date >= DateOnly.FromDateTime(dayOff.StartTime) &&
                createModel.Date <= DateOnly.FromDateTime(dayOff.EndTime))
            {
                throw new BadRequestException(
                    $"Entrance test date is in the day off range: {dayOff.StartTime:yyyy-MM-dd} and {dayOff.EndTime:yyyy-MM-dd}");
            }
        }

        var slots =
            await _unitOfWork.SlotRepository.FindProjectedAsync<SlotWithClassModel>(
                s => s.Status != SlotStatus.NotStarted && s.TeacherId == instructor.AccountFirebaseId,
                hasTrackings: false);

        if (slots.Any(s => s.Date == createModel.Date && s.Shift == createModel.Shift))
        {
            throw new BadRequestException($"Instructor {instructor.FullName ?? instructor.Email} is already busy " +
                                          $"at this time.");
        }

        entranceTest.RoomName = roomDetailModel.Name;
        entranceTest.InstructorName = instructor.FullName ?? instructor.Email;
        entranceTest.RoomCapacity = roomDetailModel.Capacity;

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.EntranceTestRepository.AddAsync(entranceTest);

            if (createModel.StudentIds.Count > 0)
            {
                await _unitOfWork.EntranceTestStudentRepository.AddRangeAsync(entranceTestStudents);

                await _unitOfWork.AccountRepository.ExecuteUpdateAsync(
                    a => createModel.StudentIds.Contains(a.AccountFirebaseId),
                    setter => setter.SetProperty(a => a.StudentStatus, StudentStatus.AttemptingEntranceTest));
            }
        });

        return await GetEntranceTestDetailById(entranceTest.Id, currentAccount);
    }

    public async Task DeleteEntranceTest(Guid id, string? currentUserFirebaseId = default)
    {
        var entranceTest = await _unitOfWork.EntranceTestRepository.GetEntranceTestWithStudentsAsync(id);

        if (entranceTest is null)
        {
            throw new NotFoundException("This EntranceTest not found.");
        }

        var status = ShiftUtils.GetEntranceTestStatus(entranceTest.Date, entranceTest.Shift);

        if (status != EntranceTestStatus.NotStarted)
        {
            throw new BadRequestException("You cannot delete this entrance test since it is already started.");
        }

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            entranceTest.DeletedById = currentUserFirebaseId;
            entranceTest.DeletedAt = DateTime.UtcNow.AddHours(7);
            entranceTest.RecordStatus = RecordStatus.IsDeleted;

            var studentIds = entranceTest.EntranceTestStudents.Select(ets => ets.StudentFirebaseId);

            await _unitOfWork.AccountRepository.ExecuteUpdateAsync(a => studentIds.Contains(a.AccountFirebaseId),
                setter => setter.SetProperty(a => a.StudentStatus, StudentStatus.WaitingForEntranceTestArrangement));

            await _unitOfWork.EntranceTestStudentRepository.ExecuteUpdateAsync(
                ets => ets.EntranceTestId == id && studentIds.Contains(ets.StudentFirebaseId),
                setter => setter.SetProperty(x => x.EntranceTestId, (Guid?)null));

            await _serviceFactory.NotificationService.SendNotificationToManyAsync(studentIds.ToList(),
                $"You have been removed from test {entranceTest.Name}", "", requiresSavingChanges: false);
        });
    }

    public async Task UpdateEntranceTest(Guid id, UpdateEntranceTestModel updateModel,
        string? currentUserFirebaseId = default)
    {
        var entranceTest = await _unitOfWork.EntranceTestRepository.FindSingleAsync(q => q.Id == id);

        if (entranceTest is null)
        {
            throw new NotFoundException("This EntranceTest not found.");
        }

        var testStatus = ShiftUtils.GetEntranceTestStatus(entranceTest.Date, entranceTest.Shift);

        if (testStatus is EntranceTestStatus.OnGoing or EntranceTestStatus.Ended)
        {
            throw new BadRequestException(
                $"Can't update since test is already {(testStatus == EntranceTestStatus.OnGoing ? "started" : "ended")}.");
        }

        updateModel.Adapt(entranceTest);

        if (updateModel.Date.HasValue || updateModel.RoomId.HasValue || updateModel.Shift.HasValue)
        {
            if (await _unitOfWork.EntranceTestRepository.AnyAsync(t => t.Date == updateModel.Date
                                                                       && t.RoomId == updateModel.RoomId
                                                                       && t.Shift == updateModel.Shift))
            {
                throw new ConflictException("There is already an entrance test with the same date, shift and room.");
            }

            var dayOffs = await _unitOfWork.DayOffRepository.GetAllAsync(hasTrackings: false);

            foreach (var dayOff in dayOffs)
            {
                if (updateModel.Date >= DateOnly.FromDateTime(dayOff.StartTime) &&
                    updateModel.Date <= DateOnly.FromDateTime(dayOff.EndTime))
                {
                    throw new BadRequestException(
                        $"Entrance test date is in the day off range: {dayOff.StartTime:yyyy-MM-dd} and {dayOff.EndTime:yyyy-MM-dd}");
                }
            }
        }

        if (!string.IsNullOrEmpty(updateModel.InstructorId))
        {
            // check Instructor is Exist in db
            var instructor =
                await _serviceFactory.AccountService.GetAccountById(updateModel.InstructorId!);

            if (instructor.Role != Role.Instructor)
            {
                throw new BadRequestException("This is not Instructor, please try again");
            }

            var slots =
                await _unitOfWork.SlotRepository.FindProjectedAsync<SlotWithClassModel>(
                    s => s.Status != SlotStatus.NotStarted && s.TeacherId == instructor.AccountFirebaseId,
                    hasTrackings: false);

            if (slots.Any(s => s.Date == entranceTest.Date && s.Shift == entranceTest.Shift))
            {
                throw new BadRequestException($"Instructor {instructor.FullName ?? instructor.Email} is already busy " +
                                              $"at this time.");
            }
        }

        if (updateModel.RoomId.HasValue)
        {
            var room = await _unitOfWork.RoomRepository.FindSingleAsync(
                r => r.Id == updateModel.RoomId.Value,
                hasTrackings: false);

            if (room is null)
            {
                throw new NotFoundException("Room not found.");
            }

            entranceTest.RoomName = room.Name;
        }

        entranceTest.Name = GetEntranceTestName(entranceTest);

        entranceTest.UpdateById = currentUserFirebaseId;
        entranceTest.UpdatedAt = DateTime.UtcNow.AddHours(7);

        List<string> accountsToPushNoti =
        [
            ..entranceTest.EntranceTestStudents.Select(ets => ets.StudentFirebaseId).ToList(),
            entranceTest.InstructorId ?? string.Empty
        ];

        await _serviceFactory.NotificationService.SendNotificationToManyAsync(accountsToPushNoti,
            message: $"Test {entranceTest.Name} information has been updated!",
            "", requiresSavingChanges: false);

        await _unitOfWork.SaveChangesAsync();
        // await _serviceFactory.RedisCacheService.SaveAsync($"entranceTest_{id}", entranceTest,
        //     TimeSpan.FromHours(5));
        // await InvalidateEntranceTestCache(id);
    }

    public async Task UpdateEntranceTestScoreAnnouncementStatus(Guid id, bool isAnnounced, AccountModel currentAccount)
    {
        var test = await _unitOfWork.EntranceTestRepository.FindSingleAsync(e => e.Id == id);
        if (test is null)
        {
            throw new NotFoundException("This EntranceTest not found.");
        }

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
            throw new BadRequestException(
                "Can't publish the score of this entrance test since not all learner test results are updated.");
        }

        var studentIds = entranceTestStudents.Select(x => x.StudentFirebaseId);

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            test.IsAnnouncedScore = isAnnounced;
            await _unitOfWork.EntranceTestStudentRepository.ExecuteUpdateAsync(ets => ets.EntranceTestId == id,
                setter => setter.SetProperty(x => x.IsScoreAnnounced,
                    isAnnounced));

            await _unitOfWork.AccountRepository.ExecuteUpdateAsync(a => studentIds.Contains(a.AccountFirebaseId),
                setter => setter.SetProperty(x => x.StudentStatus, StudentStatus.WaitingForClass));

            await _serviceFactory.NotificationService.SendNotificationToManyAsync(studentIds.ToList(),
                isAnnounced
                    ? $"Your entrance test ({test.Name}) results have been published!"
                    : $"Your entrance test ({test.Name}) results have been unpublished!", "",
                requiresSavingChanges: false);
        });
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

    public async Task AddStudentsToEntranceTest(Guid testId, AddStudentsToEntranceTestModel model,
        AccountModel currentAccount)
    {
        var entranceTest = await _unitOfWork.EntranceTestRepository.FindSingleAsync(e => e.Id == testId);

        if (entranceTest is null)
        {
            throw new NotFoundException("Entrance test not found.");
        }

        var students =
            await _unitOfWork.AccountRepository.GetStudentsWithEntranceTestStudents(
                StudentStatus.WaitingForEntranceTestArrangement,
                model.StudentIds);

        if (students.Count != model.StudentIds.Count)
        {
            throw new BadRequestException("Some of the students are not the same.");
        }

        var configs = await _serviceFactory.SystemConfigService.GetConfigs([
            ConfigNames.MinStudentsInTest, ConfigNames.MaxStudentsInTest
        ]);

        var minConfig = configs.FirstOrDefault(c => c.ConfigName == ConfigNames.MinStudentsInTest);

        var maxConfig = configs.FirstOrDefault(c => c.ConfigName == ConfigNames.MaxStudentsInTest);

        if (minConfig is not null && !string.IsNullOrEmpty(minConfig.ConfigValue))
        {
            int minStudents = Convert.ToInt32(minConfig.ConfigValue);

            if (model.StudentIds.Count +
                entranceTest.EntranceTestStudents.Count(ets => ets.EntranceTestId != null) <
                minStudents)
            {
                throw new BadRequestException($"Test must have at least {minStudents} learners");
            }
        }

        if (maxConfig is not null && !string.IsNullOrEmpty(maxConfig.ConfigValue))
        {
            int maxStudents = Convert.ToInt32(maxConfig.ConfigValue);

            if (model.StudentIds.Count +
                entranceTest.EntranceTestStudents.Count(ets => ets.EntranceTestId != null) > maxStudents)
            {
                throw new BadRequestException($"Test can only have maximum of {maxStudents} learners");
            }
        }

        var newEntranceTestStudents = new List<EntranceTestStudent>();

        foreach (var student in students)
        {
            if (entranceTest.EntranceTestStudents.Any(ets => ets.StudentFirebaseId == student.AccountFirebaseId))
            {
                throw new BadRequestException("Learner is already in test.");
            }

            var entranceTestStudent = student.EntranceTestStudents.FirstOrDefault(ets => ets.EntranceTestId == testId);

            if (entranceTestStudent is null)
            {
                newEntranceTestStudents.AddRange(students.Select(s => new EntranceTestStudent
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    CreatedById = currentAccount.AccountFirebaseId,
                    IsScoreAnnounced = entranceTest.IsAnnouncedScore,
                    StudentFirebaseId = s.AccountFirebaseId,
                    EntranceTestId = testId
                }));
            }
            else if (entranceTestStudent.EntranceTestId is null)
            {
                entranceTestStudent.EntranceTestId = testId;
            }
        }

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.AccountRepository.ExecuteUpdateAsync(a => model.StudentIds.Contains(a.AccountFirebaseId),
                setter => setter.SetProperty(x => x.StudentStatus, StudentStatus.AttemptingEntranceTest));
            await _unitOfWork.EntranceTestStudentRepository.AddRangeAsync(newEntranceTestStudents);
        });
    }

    public async Task RemoveStudentsFromTest(Guid testId, AccountModel currentAccount, params List<string> studentIds)
    {
        var test = await _unitOfWork.EntranceTestRepository.FindSingleAsync(e => e.Id == testId);

        if (test is null)
        {
            throw new NotFoundException("Entrance test not found.");
        }

        var testStatus = ShiftUtils.GetEntranceTestStatus(test.Date, test.Shift);

        if (testStatus != EntranceTestStatus.NotStarted)
        {
            throw new BadRequestException("Entrance test is already started.");
        }

        var entranceTestStudents =
            await _unitOfWork.EntranceTestStudentRepository.FindAsync(ets => ets.EntranceTestId == testId);

        if (entranceTestStudents.Count == 0)
        {
            throw new BadRequestException("Invalid entrance test");
        }

        var minStudentsConfig =
            await _unitOfWork.SystemConfigRepository.FindFirstAsync(c => c.ConfigName == ConfigNames.MinStudentsInTest);

        if (minStudentsConfig is not null)
        {
            var minStudents = !string.IsNullOrEmpty(minStudentsConfig.ConfigValue)
                ? Convert.ToInt32(minStudentsConfig.ConfigValue)
                : 1;

            if (entranceTestStudents.Count - studentIds.Count < minStudents)
            {
                throw new BadRequestException($"Entrance test must have at least {minStudents} learners");
            }
        }

        var studentIdsInTest = entranceTestStudents.Select(ets => ets.StudentFirebaseId).ToList();

        foreach (var studentId in studentIds)
        {
            if (!studentIdsInTest.Contains(studentId))
            {
                throw new BadRequestException("Invalid student");
            }
        }

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.EntranceTestStudentRepository.ExecuteUpdateAsync(ets => ets.EntranceTestId == testId
                    && studentIds.Contains(ets.StudentFirebaseId),
                setter => setter.SetProperty(x => x.EntranceTestId, (Guid?)null)
                    .SetProperty(x => x.DeletedAt, DateTime.UtcNow.AddHours(7))
                    .SetProperty(x => x.DeletedById, currentAccount.AccountFirebaseId));

            await _unitOfWork.AccountRepository.ExecuteUpdateAsync(a => studentIds.Contains(a.AccountFirebaseId),
                setter => setter.SetProperty(x => x.StudentStatus, StudentStatus.WaitingForEntranceTestArrangement));

            await _serviceFactory.NotificationService.SendNotificationToManyAsync(studentIds.ToList(),
                $"You have been removed from entrance test", "", requiresSavingChanges: false);
        });
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

        if (entranceTest is null)
        {
            throw new NotFoundException("Entrance test not found");
        }

        if (entranceTest.EntranceTestStudents.Count <= 1)
        {
            throw new BadRequestException("Only 1 student is in the entrance test.");
        }

        var testStatus = ShiftUtils.GetEntranceTestStatus(entranceTest.Date, entranceTest.Shift);

        if (testStatus != EntranceTestStatus.NotStarted)
        {
            throw new BadRequestException("Entrance test has already been started.");
        }

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            entranceTestStudent.RecordStatus = RecordStatus.IsDeleted;
            entranceTestStudent.DeletedAt = DateTime.UtcNow.AddHours(7);
            entranceTestStudent.DeletedById = currentAccount.AccountFirebaseId;

            await _unitOfWork.AccountRepository.ExecuteUpdateAsync(a => a.AccountFirebaseId == studentId,
                setter => setter.SetProperty(x => x.StudentStatus, StudentStatus.WaitingForEntranceTestArrangement));
        });
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
                ? -Convert.ToInt32(feeConfig.ConfigValue)
                : -100_000,
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
                    CreatedAt = DateTime.UtcNow.AddHours(7),
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
                        $"Learner {account.FullName} has just registered for entrance test", "");
                });

                break;
            case PaymentStatus.Failed:
                throw new BadRequestException("Payment has failed.");
            default:
                throw new BadRequestException("Unknown payment status.");
        }
    }

    private async Task<(List<EntranceTest> newEntranceTests, List<EntranceTestWithStudentsModel> existingEntranceTests)>
        CreateAndAssignStudentsToEntranceTests(
            List<AccountDetailModel> students,
            DateTime startDate, DateTime? endDate,
            string staffAccountId, List<EntranceTestStudent> entranceTestStudents, params List<Shift> shiftOptions
        )
    {
        var maxStudentsPerSlotConfig =
            await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.MaxStudentsInTest);
        int maxStudentsPerSlot = Convert.ToInt32(maxStudentsPerSlotConfig.ConfigValue);

        var entranceTests = new List<EntranceTest>();
        var remainingStudents = new List<AccountDetailModel>(students);
        // var addingEntranceTestStudents = new List<EntranceTestStudent>();

        var existingTests = await _unitOfWork.EntranceTestRepository.FindProjectedAsync<EntranceTestWithStudentsModel>(
            e => e.Date >= DateOnly.FromDateTime(startDate)
                 && (!endDate.HasValue || e.Date <= DateOnly.FromDateTime(endDate.Value)),
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

                            var entranceTestStudent = entranceTestStudents.FirstOrDefault(ets =>
                                ets.StudentFirebaseId == student.AccountFirebaseId)!;

                            entranceTestStudent.EntranceTestId = entranceTest.Id;
                            entranceTestStudent.FullName = student.FullName ?? student.Email;


                            // var newEntranceTestStudent = new EntranceTestStudent
                            // {
                            //     Id = Guid.NewGuid(),
                            //     StudentFirebaseId = student.AccountFirebaseId,
                            //     EntranceTestId = entranceTest.Id,
                            //     CreatedById = staffAccountId,
                            //     FullName = student.FullName,
                            //     CreatedAt = DateTime.UtcNow,
                            // };
                            //
                            // entranceTest.EntranceTestStudents.Add(
                            //     newEntranceTestStudent.Adapt<EntranceTestStudentModel>());
                            // addingEntranceTestStudents.Add(newEntranceTestStudent);
                            entranceTest.EntranceTestStudents.Add(entranceTestStudent
                                .Adapt<EntranceTestStudentModel>());
                            assignedStudents.Add(student.AccountFirebaseId);
                        }
                    }
                }

                //Remove assigned students from the remaining list
                remainingStudents.RemoveAll(s => assignedStudents.Contains(s.AccountFirebaseId));

                //Step 2: If there are still unassigned students, create a new entrance test
                if (remainingStudents.Count == 0)
                {
                    break;
                }

                var entranceTestId = Guid.NewGuid();
                var studentsToAssign = remainingStudents
                    .Take(Math.Min(room.Capacity ?? 10, maxStudentsPerSlot))
                    .ToList();

                var studentIdsToAssign = studentsToAssign.Select(s => s.AccountFirebaseId);

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
                    // EntranceTestStudents = studentsToAssign.Select(student => new EntranceTestStudent
                    // {
                    //     Id = Guid.NewGuid(),
                    //     StudentFirebaseId = student.AccountFirebaseId,
                    //     CreatedById = staffAccountId,
                    //     FullName = student.FullName,
                    //     CreatedAt = DateTime.UtcNow,
                    // }).ToList()
                };

                foreach (var ets in entranceTestStudents.Where(
                             ets => studentIdsToAssign.Contains(ets.StudentFirebaseId)))
                {
                    ets.EntranceTestId = newEntranceTest.Id;
                }

                entranceTests.Add(newEntranceTest);
                remainingStudents.RemoveAll(s => assignedStudents.Contains(s.AccountFirebaseId));
            }
        }

        // await _unitOfWork.EntranceTestStudentRepository.AddRangeAsync(addingEntranceTestStudents);
        // await _unitOfWork.SaveChangesAsync();

        return (entranceTests, existingTests);
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
            .FindAsync(e => e.Date >= DateOnly.FromDateTime(startDate)
                            && (!endDate.HasValue || e.Date <= DateOnly.FromDateTime(endDate.Value)),
                hasTrackings: false);

        var entranceTestStudents = await _unitOfWork.EntranceTestStudentRepository.FindAsync(ets =>
            ets.EntranceTestId == null
            && model.StudentIds.Contains(ets.StudentFirebaseId));

        var (entranceTests, tests) = await CreateAndAssignStudentsToEntranceTests(students,
            startDate, endDate,
            currentAccount.AccountFirebaseId, entranceTestStudents, shiftOptions);

        if (entranceTests.Count > 0)
        {
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
        }

        var arrangedStudentIds = students.Select(s => s.AccountFirebaseId);

        List<Task> notiTasks = [];

        if (entranceTests.Count > 0)
        {
            foreach (var test in entranceTests)
            {
                test.Name = GetEntranceTestName(test);

                var studentIdsToPushNotification =
                    test.EntranceTestStudents.Select(ets => ets.StudentFirebaseId).ToList();

                notiTasks.Add(_serviceFactory.NotificationService.SendNotificationToManyAsync(
                    studentIdsToPushNotification,
                    $"You have been arranged into test {test.Name}", "", requiresSavingChanges: false));
            }
        }

        if (tests.Count > 0)
        {
            foreach (var test in tests)
            {
                var studentIdsToPushNotification =
                    test.EntranceTestStudents.Select(ets => ets.StudentFirebaseId).ToList();

                notiTasks.Add(_serviceFactory.NotificationService.SendNotificationToManyAsync(
                    studentIdsToPushNotification,
                    $"You have been arranged into test {test.Name}", "", requiresSavingChanges: false));
            }
        }

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.EntranceTestRepository.AddRangeAsync(entranceTests);

            await _unitOfWork.AccountRepository.ExecuteUpdateAsync(
                expression: a => arrangedStudentIds.Contains(a.AccountFirebaseId),
                setter => setter.SetProperty(a => a.StudentStatus, StudentStatus.AttemptingEntranceTest));
            await Task.WhenAll(notiTasks);
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
        var now = DateTime.UtcNow.AddHours(7);
        var today = DateOnly.FromDateTime(now);

        if (today > dateToCompare)
        {
            return true;
        }

        if (today == dateToCompare)
        {
            var shiftEndTime = GetShiftEndTime(shift);
            var shiftEndDateTime = dateToCompare.ToDateTime(shiftEndTime);

            return now > shiftEndDateTime;
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

        var criteriaIds = updateModel.UpdateScoreRequests.Select(s => s.CriteriaId);

        List<Criteria> criterias = [];

        if (updateModel.UpdateScoreRequests.Count > 0)
        {
            criterias =
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

                decimal newPracticalScore = updateModel.UpdateScoreRequests.Aggregate(decimal.Zero, (current, result) =>
                    current + result.Score * (criterias.FirstOrDefault(c => c.Id == result.CriteriaId)!.Weight / 100)
                );

                entranceTestStudent.LevelId = await _serviceFactory.LevelService.GetLevelIdFromScores(
                    Convert.ToDecimal(entranceTestStudent.TheoraticalScore ?? 0), newPracticalScore);

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