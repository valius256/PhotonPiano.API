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
        var (page, pageSize, sortColumn, orderByDesc,
                roomIds, keyword, shifts,
                entranceTestIds, isAnnouncedScore, isOpen,
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
                    e => isOpen == null || e.IsOpen == isOpen.Value,
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

    private async Task InvalidateEntranceTestCache(Guid? id = null)
    {
        // Invalidate general cache
        await _serviceFactory.RedisCacheService.DeleteAsync("entranceTests");

        if (id.HasValue)
            // Invalidate specific cache for the entrance test
            await _serviceFactory.RedisCacheService.DeleteAsync($"entranceTest_{id.Value}");
    }
}