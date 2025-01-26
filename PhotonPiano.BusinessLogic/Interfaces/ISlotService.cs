using PhotonPiano.BusinessLogic.BusinessModel.Slot;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ISlotService
{
    Task<List<SlotDetailModel>> GetSlotsAsync(GetSlotModel slotModel, string? userFirebaseid = default);

    Task<SlotDetailModel> GetSLotDetailById(Guid id, string? userFirebaseid = default);

    TimeOnly GetShiftStartTime(Shift shift);
}