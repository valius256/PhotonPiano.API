using Mapster;
using Microsoft.EntityFrameworkCore;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
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
        var isRoomExisted = await _serviceFactory.RoomService.IsRoomExist(entranceTestModel.RoomId!);
        // check Instructor is Exist in db
        var isInstructorExisted = await _serviceFactory.AccountService.IsAccountExist(entranceTestModel.InstructorId!);

        var createdEntranceTestId = Guid.Empty;
        if (isRoomExisted && isInstructorExisted)
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var createdEntranceTestEntity = await _unitOfWork.EntranceTestRepository.AddAsync(entranceTestModel);
                createdEntranceTestId = createdEntranceTestEntity.Id;
            });

        return await GetEntranceTestDetailById(createdEntranceTestId);
    }

    public async Task DeleteEntranceTest(Guid id, string? currentUserFirebaseId = default)
    {
        var entranceTestEntity = await _unitOfWork.EntranceTestRepository.FindSingleAsync(q => q.Id == id);
        if (entranceTestEntity is null) throw new NotFoundException("This EntranceTest not found.");

        entranceTestEntity.DeletedById = currentUserFirebaseId;
        entranceTestEntity.DeletedAt = DateTime.UtcNow.AddHours(7);
        entranceTestEntity.RecordStatus = RecordStatus.IsDeleted;

        await _unitOfWork.SaveChangesAsync();
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
    }
}