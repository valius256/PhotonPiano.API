using Mapster;
using Microsoft.EntityFrameworkCore;
using PhotonPiano.BusinessLogic.BusinessModel.Room;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services;

public class RoomService : IRoomService
{
    private readonly IUnitOfWork _unitOfWork;

    public RoomService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<RoomDetailModel>> GetPagedRooms(QueryRoomModel query)
    {
        var (page, pageSize, sortColumn, orderByDesc, keyword, roomStatuses)
            = query;

        var likeKeyword = query.GetLikeKeyword();

        var result = await _unitOfWork.RoomRepository.GetPaginatedWithProjectionAsync<RoomDetailModel>(page, pageSize,
            sortColumn, orderByDesc,
            expressions:
            [
                r => roomStatuses != null && (roomStatuses.Count == 0 || roomStatuses.Contains(r.Status)),

                // search 
                r => string.IsNullOrEmpty(keyword) ||
                     EF.Functions.ILike(EF.Functions.Unaccent(r.Name ?? string.Empty), likeKeyword)
            ]);

        return result;
    }
    public async Task<List<RoomModel>> GetAvailableRooms(Shift shift, List<DayOfWeek> dayOfTheWeeks, DateOnly startWeek, int totalDays)
    {
        //TODO : Consider day-offs
        if (startWeek.DayOfWeek != DayOfWeek.Monday)
        {
            throw new BadRequestException("Incorrect start week");
        }
        var dates = new List<DateOnly>();
        while (totalDays > 0)
        {
            foreach (var dayOfWeek in dayOfTheWeeks)
            {
                dates.Add(startWeek.AddDays((int)dayOfWeek));
                totalDays--;
                if (totalDays == 0)
                {
                    break;
                }
            }
            startWeek = startWeek.AddDays(8);
        }
        var rooms = new List<RoomModel>();
        foreach (var date in dates)
        {
            var availableRooms = await _unitOfWork.RoomRepository.FindAsync(r => !r.Slots.Any(s => s.Shift != shift && s.Date == date));
            availableRooms.ForEach(ar =>
            {
                if (!rooms.Any(r => r.Id == ar.Id))
                {
                    rooms.Add(ar.Adapt<RoomModel>());
                }
            });
        }

        return rooms;
    }

    public async Task<RoomDetailModel> GetRoomDetailById(Guid id)
    {
        var result = await _unitOfWork.RoomRepository
            .FindSingleProjectedAsync<RoomDetailModel>(e => e.Id == id, false);
        if (result is null) throw new NotFoundException("Room not found.");
        return result;
    }


    public async Task<RoomDetailModel> CreateRoom(RoomModel roomModel, string? currentUserFirebaseId = default)
    {
        var roomMappedEntity = roomModel.Adapt<Room>();
        roomMappedEntity.CreatedById = currentUserFirebaseId!;

        var createdRoomId = Guid.Empty;

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var createdRoom = await _unitOfWork.RoomRepository.AddAsync(roomMappedEntity);
            createdRoomId = createdRoom.Id;
        });

        return await GetRoomDetailById(createdRoomId);
    }

    public async Task DeleteRoom(Guid id, string? currentUserFirebaseId = default)
    {
        var entranceTestEntity = await _unitOfWork.RoomRepository.FindSingleAsync(q => q.Id == id);
        if (entranceTestEntity is null) throw new NotFoundException($"This Room with ID {id} not found.");

        entranceTestEntity.DeletedById = currentUserFirebaseId;
        entranceTestEntity.DeletedAt = DateTime.UtcNow.AddHours(7);
        entranceTestEntity.RecordStatus = RecordStatus.IsDeleted;

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateRoom(Guid id, UpdateRoomModel roomModel, string? currentUserFirebaseId = default)
    {
        var roomEntity = await _unitOfWork.RoomRepository.FindSingleAsync(q => q.Id == id);

        if (roomEntity is null) throw new NotFoundException($"This Room with ID {id} not found.");

        roomModel.Adapt(roomEntity);

        roomEntity.UpdateById = currentUserFirebaseId;
        roomEntity.UpdatedAt = DateTime.UtcNow.AddHours(7);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> IsRoomExist(Guid id)
    {
        var result = await _unitOfWork.RoomRepository
            .FindSingleProjectedAsync<RoomDetailModel>(e => e.Id == id, false);
        if (result is null) throw new NotFoundException("Room not found.");
        return true;
    }
}