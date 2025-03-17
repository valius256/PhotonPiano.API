using System.ComponentModel.DataAnnotations;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Slot;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ISlotService
{
    Task<List<SlotDetailModel>> GetSlotsAsync(GetSlotModel slotModel, AccountModel? accountModel);

    Task<SlotDetailModel> GetSLotDetailById(Guid id, AccountModel? accountModel = default);

    TimeOnly GetShiftStartTime(Shift shift);


    Task<List<SlotSimpleModel>> GetWeeklyScheduleAsync(GetSlotModel slotModel, [Required] AccountModel accountModel);

    Task<List<StudentAttendanceModel>> GetAttendanceStatusAsync(Guid slotId);
    
    Task CronAutoChangeSlotStatus();

    Task<SlotModel> CreateSlot(CreateSlotModel createSlotModel, string accountFirebaseId);

    Task<SlotModel> UpdateSlot(UpdateSlotModel updateSlotModel, string accountFirebaseId);

    Task DeleteSlot(Guid slotId, string accountFirebaseId);
}