using Mapster;
using Microsoft.EntityFrameworkCore;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services
{
    public class EntranceTestService : IEntranceTestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceFactory _serviceFactory;

        public EntranceTestService(IUnitOfWork unitOfWork, IServiceFactory serviceFactory)
        {
            _unitOfWork = unitOfWork;
            _serviceFactory = serviceFactory;
        }


        public async Task<PagedResult<EntranceTestDetailModel>> GetPagedEntranceTest(QueryEntranceTestModel query)
        {
            var (page, pageSize, sortColumn, orderByDesc, roomIds,
                    keyword, shifts, entranceTestIds, isAnnouncedScore, instructorIds)
                = query;

            string likeKeyword = query.GetLikeKeyword();

            var result = await _unitOfWork.EntranceTestRepository
                .GetPaginatedWithProjectionAsync<EntranceTestDetailModel>(
                    page, pageSize, sortColumn, orderByDesc,
                    expressions:
                    [
                        e => roomIds != null && (roomIds.Count == 0 || roomIds.Contains(e.RoomId)),
                        e =>  e.IsAnnouncedTime == isAnnouncedScore,
                        e => shifts != null && (shifts.Count == 0 || shifts.Contains(e.Shift)),
                        e => instructorIds != null && (instructorIds.Count == 0 || instructorIds.Contains(e.InstructorId)),
                        e => entranceTestIds != null && (entranceTestIds.Count == 0 || entranceTestIds.Contains(e.Id)),
                        
                        // search 
                        e => (string.IsNullOrEmpty(keyword) ||
                                                    EF.Functions.ILike(EF.Functions.Unaccent(e.RoomName?? string.Empty), likeKeyword) ||
                                                    EF.Functions.ILike(EF.Functions.Unaccent(e.InstructorName ?? string.Empty), likeKeyword))

                    ]);

            return result;
        }

        public async Task<EntranceTestDetailModel> GetEntranceTestDetailById(Guid id)
        {
            var result = await _unitOfWork.EntranceTestRepository
                .FindSingleProjectedAsync<EntranceTestDetailModel>(e => e.Id == id, hasTrackings: false);
            if (result is null)
            {
                throw new NotFoundException("EntranceTest not found.");
            }
            return result;
        }

        public async Task<EntranceTestDetailModel> CreateEntranceTest(EntranceTestModel entranceTestStudent, string? currentUserFirebaseId = default)
        {
            var entranceTestModel = entranceTestStudent.Adapt<EntranceTest>();
            entranceTestModel.CreatedById = currentUserFirebaseId!;

            // check room is exist 
            var roomEntity = await _serviceFactory.RoomService.GetRoomDetailById(entranceTestModel.RoomId!);
             if (roomEntity is null) throw new BadRequestException("Room is not exist please enter a valid room");

            // check Instructor is Exist in db
            var instructorEntity =
                await _serviceFactory.AccountService.GetAccountById(entranceTestModel.InstructorId!);
             if (instructorEntity is null) throw new BadRequestException("Instructor is not exist please enter a valid instructor ");


            var createdEntranceTestId = Guid.Empty;
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
            if (entranceTestEntity is null)
            {
                throw new NotFoundException("This entranceTestStudent not found.");
            }

            entranceTestEntity.DeletedById = currentUserFirebaseId;
            entranceTestEntity.DeletedAt = DateTime.UtcNow.AddHours(7);
            entranceTestEntity.RecordStatus = RecordStatus.IsDeleted;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateEntranceTest(Guid id, UpdateEntranceTestModel entranceTestStudentModel,
            string? currentUserFirebaseId = default)
        {
            var entranceTestEntity = await _unitOfWork.EntranceTestRepository.FindSingleAsync(q => q.Id == id);

            if (entranceTestEntity is null)
            {
                throw new NotFoundException("This entranceTestStudent not found.");
            }

            entranceTestStudentModel.Adapt(entranceTestEntity);

            entranceTestEntity.UpdateById = currentUserFirebaseId;
            entranceTestEntity.UpdatedAt = DateTime.UtcNow.AddHours(7);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
