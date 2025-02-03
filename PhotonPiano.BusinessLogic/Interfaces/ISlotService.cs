using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Slot;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ISlotService
{
    Task<List<SlotDetailModel>> GetSlotsAsync(GetSlotModel slotModel, AccountModel? accountModel);

    Task<SlotDetailModel> GetSLotDetailById(Guid id, string? userFirebaseid = default);

    TimeOnly GetShiftStartTime(Shift shift);


    Task<List<SlotSimpleModel>> GetWeeklyScheduleAsync(GetSlotModel slotModel, string? userFirebaseId = default);

    Task<List<StudentAttendanceModel>> GetAttendanceStatusAsync(Guid slotId);
}