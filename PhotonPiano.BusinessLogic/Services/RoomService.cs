using Mapster;
using Microsoft.EntityFrameworkCore;
using PhotonPiano.BusinessLogic.BusinessModel.FreeSlot;
using PhotonPiano.BusinessLogic.BusinessModel.Room;
using PhotonPiano.BusinessLogic.BusinessModel.Utils;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;
using PhotonPiano.Shared.Enums;
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
    public async Task<List<RoomModel>> GetAvailableRooms(List<(DateOnly, Shift)> timeSlots, List<Slot> otherSlots)
    {
        var dates = timeSlots.Select(ts => ts.Item1).ToHashSet();
        var shifts = timeSlots.Select(ts => ts.Item2).ToHashSet();

        var bookedRoomIds = await _unitOfWork.SlotRepository.Entities
            .Where(s => dates.Contains(s.Date) && shifts.Contains(s.Shift))
            .Select(s => s.RoomId)
            .Distinct()
            .ToListAsync();

        // Get booked room IDs from the additional (in-memory) slots
        var newlyBookedRoomIds = otherSlots
            .Where(s => timeSlots.Any(ts => ts.Item1 == s.Date && ts.Item2 == s.Shift))
            .Select(s => s.RoomId)
            .Distinct()
            .ToList();

        var bookRoomIdsForEntranceTest = await _unitOfWork.EntranceTestRepository.Entities
            .Where(et => dates.Contains(et.Date) && shifts.Contains(et.Shift))
            .Select(et => (Guid?)et.RoomId)
            .Distinct()
            .ToListAsync();

        var allBookedRoomIds = bookedRoomIds
            .Union(newlyBookedRoomIds)
            .Union(bookRoomIdsForEntranceTest)
            .ToHashSet();

        // Get available rooms (not in booked room IDs)
        var availableRooms = await _unitOfWork.RoomRepository.Entities
            .Where(r => !allBookedRoomIds.Contains(r.Id) && r.Status == RoomStatus.Opened)
            .ToListAsync();

        // Convert to RoomModel, ensuring unique rooms using HashSet
        return [.. availableRooms
            .DistinctBy(r => r.Id) // LINQ to remove duplicates
            .Select(r => r.Adapt<RoomModel>())];
        
    }


    public async Task<RoomDetailModel> GetRoomDetailById(Guid id)
    {
        var result = await _unitOfWork.RoomRepository
            .FindFirstProjectedAsync<RoomDetailModel>(e => e.Id == id, false);
        if (result is null || result.Id == new Guid())
        {
            throw new NotFoundException("Room not found.");
        }
        return result;
    }


    public async Task<RoomDetailModel> CreateRoom(RoomModel roomModel, string? currentUserFirebaseId = default)
    {
        var roomMappedEntity = roomModel.Adapt<Room>();
        roomMappedEntity.CreatedById = currentUserFirebaseId!;
        
        if(roomMappedEntity.Capacity <= 0)
        {
            throw new BadRequestException("Room capacity must be greater than 0.");
        }
        
        var roomWithSameName = await _unitOfWork.RoomRepository
            .FindSingleAsync(r => r.Name == roomMappedEntity.Name);

        if (roomWithSameName is not null)
        {
            throw new BadRequestException("Room with same name already exists.");
        }
        
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
        var roomEntity = await _unitOfWork.RoomRepository.FindSingleAsync(q => q.Id == id);
        if (roomEntity is null) throw new NotFoundException($"This Room with ID {id} not found.");
    
        // Check if room has any slots using it
        var hasSlots = await _unitOfWork.SlotRepository.Entities
            .AnyAsync(s => s.RoomId == id);
        
        // Check if room has any entrance tests using it
        var hasEntranceTests = await _unitOfWork.EntranceTestRepository.Entities
            .AnyAsync(et => et.RoomId == id);
    
        if (hasSlots || hasEntranceTests)
            throw new InvalidOperationException("Cannot delete room that is being used by learning slots or entrance tests.");
    
        roomEntity.DeletedById = currentUserFirebaseId;
        roomEntity.DeletedAt = DateTime.UtcNow.AddHours(7);
        roomEntity.RecordStatus = RecordStatus.IsDeleted;

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateRoom(Guid id, UpdateRoomModel roomModel, string? currentUserFirebaseId = default)
    {
        var roomEntity = await _unitOfWork.RoomRepository.FindSingleAsync(q => q.Id == id);

        if (roomEntity is null) throw new NotFoundException($"This Room with ID {id} not found.");
        
        if(roomEntity.Capacity <= 0)
        {
            throw new BadRequestException("Room capacity must be greater than 0.");
        }
        
        var roomWithSameName = await _unitOfWork.RoomRepository
            .FindSingleAsync(r => r.Name == roomEntity.Name);

        if (roomWithSameName is not null)
        {
            throw new BadRequestException("Room with same name already exists.");
        }

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