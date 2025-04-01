
using PhotonPiano.BusinessLogic.BusinessModel.FreeSlot;

namespace PhotonPiano.BusinessLogic.Interfaces
{
    public interface IFreeSlotService
    {
        Task<List<FreeSlotModel>> GetFreeSlots(string accountFirebaseId);
        Task UpsertFreeSlots(List<CreateFreeSlotModel> freeSlotModels, string userFirebaseId);
    }
}
