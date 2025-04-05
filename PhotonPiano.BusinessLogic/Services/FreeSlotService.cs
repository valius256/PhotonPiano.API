
using Mapster;
using PhotonPiano.BusinessLogic.BusinessModel.FreeSlot;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.BusinessLogic.Services
{
    public class FreeSlotService : IFreeSlotService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FreeSlotService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<FreeSlotModel>> GetFreeSlots(string accountFirebaseId)
        {
            return (await _unitOfWork.FreeSlotRepository.FindAsync(f => f.AccountId == accountFirebaseId,false))
                .Adapt<List<FreeSlotModel>>();
        }

        public async Task UpsertFreeSlots(List<CreateFreeSlotModel> freeSlotModels, string userFirebaseId)
        {
            var requestedSlots = freeSlotModels.Adapt<List<FreeSlot>>();
            var slots = await GetFreeSlots(userFirebaseId);

            var slotToAdd = requestedSlots.Where(rs => !slots.Any(s => s.DayOfWeek == rs.DayOfWeek && s.Shift == rs.Shift)).ToList();
            var slotToDelete = slots.Where(s => !requestedSlots.Any(rs => s.DayOfWeek == rs.DayOfWeek && s.Shift == rs.Shift)).ToList();

            foreach (var slot in slotToAdd)
            {
                slot.AccountId = userFirebaseId;
            }
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _unitOfWork.FreeSlotRepository.AddRangeAsync(slotToAdd);
                await _unitOfWork.FreeSlotRepository.DeleteRangeAsync(slotToDelete.Adapt<List<FreeSlot>>());
                await _unitOfWork.SaveChangesAsync();
            });
        }
    }
}
