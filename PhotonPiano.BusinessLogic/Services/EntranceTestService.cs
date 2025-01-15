using Mapster;
using Microsoft.EntityFrameworkCore;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;
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


    public async Task<PagedResult<EntranceTestDetailModel>> GetPagedEntranceTest(QueryEntranceTestModel query)
    {
        var cache =
            await _serviceFactory.RedisCacheService.GetAsync<PagedResult<EntranceTestDetailModel>>("entranceTests");
        if (cache is not null) return cache;

        var (page, pageSize, sortColumn, orderByDesc,
                roomIds, keyword, shifts,
                entranceTestIds, isAnnouncedScore, isOpen,
                instructorIds)
            = query;

        var likeKeyword = query.GetLikeKeyword();

        var result = await _unitOfWork.EntranceTestRepository
            .GetPaginatedWithProjectionAsync<EntranceTestDetailModel>(
                page, pageSize, sortColumn, orderByDesc,
                expressions:
                [
                    e => roomIds != null && (roomIds.Count == 0 || roomIds.Contains(e.RoomId)),
                    e => isOpen == null || e.IsOpen == isOpen.Value,
                    e => isAnnouncedScore == null || e.IsAnnouncedScore == isAnnouncedScore.Value,
                    e => shifts != null && (shifts.Count == 0 || shifts.Contains(e.Shift)),
                    e => instructorIds != null &&
                         (instructorIds.Count == 0 || instructorIds.Contains(e.InstructorId)),
                    e => entranceTestIds != null && (entranceTestIds.Count == 0 || entranceTestIds.Contains(e.Id)),

                    // search 
                    e => string.IsNullOrEmpty(keyword) ||
                         EF.Functions.ILike(EF.Functions.Unaccent(e.RoomName ?? string.Empty), likeKeyword) ||
                         EF.Functions.ILike(EF.Functions.Unaccent(e.InstructorName ?? string.Empty), likeKeyword)
                ]);

        await _serviceFactory.RedisCacheService.SaveAsync("entranceTests", result, TimeSpan.FromHours(3));
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

    public async Task<EntranceTestDetailModel> CreateEntranceTest(EntranceTestModel entranceTestStudent,
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

        EntranceTest createdEntranceTest = null;
        entranceTestModel.RoomName = roomDetailModel.Name;
        entranceTestModel.InstructorName = instructorDetailModel.Name;

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var createdEntranceTestEntity = await _unitOfWork.EntranceTestRepository.AddAsync(entranceTestModel);
            createdEntranceTest = createdEntranceTestEntity;
        });

        await _serviceFactory.RedisCacheService.DeleteAsync("entranceTests");
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

        await _serviceFactory.RedisCacheService.DeleteAsync("entranceTests");
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

        await _serviceFactory.RedisCacheService.DeleteAsync("entranceTests");
    }

    public async Task<PagedResult<EntranceTestStudentDetail>> GetPagedEntranceTestStudent(QueryPagedModel query,
        Guid entranceTestId, AccountModel currentAccount)
    {
        if (!await _unitOfWork.EntranceTestRepository.AnyAsync(et => et.Id == entranceTestId))
        {
            throw new NotFoundException("Entrance test not found.");
        }

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
                    ets => currentAccount.Role != Role.Student || ets.StudentFirebaseId == currentAccount.AccountFirebaseId
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
            throw new NotFoundException("Entrance test student not found");
        }

        if (currentAccount.Role == Role.Student &&
            entranceTestStudent.StudentFirebaseId != currentAccount.AccountFirebaseId)
        {
            throw new ForbiddenMethodException("You are not allowed to access this resource.");
        }

        return entranceTestStudent;
    }
}