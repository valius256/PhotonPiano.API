using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Slot;
using PhotonPiano.DataAccess.Models.Enum;
using System.ComponentModel.DataAnnotations;
using PhotonPiano.Shared.Enums;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ISlotService
{
    Task<List<SlotDetailModel>> GetSlots(GetSlotModel slotModel, AccountModel? accountModel);

    Task<SlotDetailModel> GetSlotDetailById(Guid id, AccountModel? accountModel = default);

    TimeOnly GetShiftStartTime(Shift shift);


    Task<List<SlotDetailModel>> GetWeeklySchedule(GetSlotModel slotModel, [Required] AccountModel accountModel);

    Task<List<StudentAttendanceModel>> GetAttendanceStatus(Guid slotId);

    Task CronAutoChangeSlotStatus();

    Task<SlotModel> CreateSlot(CreateSlotModel createSlotModel, string accountFirebaseId);

    Task<SlotModel> UpdateSlot(UpdateSlotModel updateSlotModel, string accountFirebaseId);

    Task DeleteSlot(Guid slotId, string accountFirebaseId);

    Task<List<BlankSlotModel>> GetAllBlankSlotInWeeks(DateOnly? startDate, DateOnly? endDate);

    Task<bool> CancelSlot(CancelSlotModel model, string accountFirebaseId);

    Task<SlotDetailModel> PublicNewSlot(PublicNewSlotModel model, string accountFirebaseId);
    
    Task<List<AccountSimpleModel>> GetAllTeacherCanBeAssignedToThisSlot(Guid slotId, string accountFirebaseId);
    
    Task<SlotDetailModel> AssignTeacherToSlot(Guid slotId, string teacherFirebaseId, string reason ,string staffAccountFirebaseId);
}