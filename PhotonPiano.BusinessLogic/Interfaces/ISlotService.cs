using PhotonPiano.BusinessLogic.BusinessModel.Slot;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ISlotService
{
    Task<List<SlotDetailModel>> GetSlotsAsync(GetSlotModel slotModel, string? userFirebaseid = default);
}