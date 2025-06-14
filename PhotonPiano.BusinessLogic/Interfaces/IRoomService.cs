using PhotonPiano.BusinessLogic.BusinessModel.Room;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;
using PhotonPiano.Shared.Enums;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface IRoomService
{
    Task<PagedResult<RoomDetailModel>> GetPagedRooms(QueryRoomModel query);

    Task<RoomDetailModel> GetRoomDetailById(Guid id);

    Task<RoomDetailModel> CreateRoom(RoomModel roomModel, string? currentUserFirebaseId = default);

    Task DeleteRoom(Guid id, string? currentUserFirebaseId = default);

    Task UpdateRoom(Guid id, UpdateRoomModel roomModel, string? currentUserFirebaseId = default);

    Task<bool> IsRoomExist(Guid id);

    Task<List<RoomModel>> GetAvailableRooms(List<(DateOnly, Shift)> timeSlots, List<Slot> otherSlots);
}