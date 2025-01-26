using PhotonPiano.BusinessLogic.BusinessModel.Slot;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Exceptions;
using System.Linq.Expressions;

namespace PhotonPiano.BusinessLogic.Services;

public class SlotService : ISlotService
{
    private readonly IServiceFactory _serviceFactory;
    private readonly IUnitOfWork _unitOfWork;

    public SlotService(IUnitOfWork unitOfWork, IServiceFactory serviceFactory)
    {
        _unitOfWork = unitOfWork;
        _serviceFactory = serviceFactory;
    }

    public TimeOnly GetShiftStartTime(Shift shift)
    {
        var shiftStartTimes = new Dictionary<Shift, TimeOnly>
    {
        { Shift.Shift1_7h_8h30, new TimeOnly(7, 0) },
        { Shift.Shift2_8h45_10h15, new TimeOnly(8, 45) },
        { Shift.Shift3_10h45_12h, new TimeOnly(10, 45) },
        { Shift.Shift4_12h30_14h00, new TimeOnly(12, 30) },
        { Shift.Shift5_14h15_15h45, new TimeOnly(14, 15) },
        { Shift.Shift6_16h00_17h30, new TimeOnly(16, 0) },
        { Shift.Shift7_18h_19h30, new TimeOnly(18, 0) },
        { Shift.Shift8_19h45_21h15, new TimeOnly(19, 45) }
    };

        if (!shiftStartTimes.TryGetValue(shift, out var shiftStartTime))
        {
            throw new InvalidOperationException("Invalid shift value.");
        }

        return shiftStartTime;
    }

    public async Task<SlotDetailModel> GetSLotDetailById(Guid id, string? userFirebaseid = null)
    {
        var result = await _unitOfWork.SlotRepository.FindSingleProjectedAsync<SlotDetailModel>(
            expression: s => s.Id == id,
            hasTrackings: false
        );

        if (result == null)
        {
            throw new NotFoundException("Slot not found");
        }

        return result;
    }

    public async Task<List<SlotDetailModel>> GetSlotsAsync(GetSlotModel slotModel, string? userFirebaseId = default)
    {
        Expression<Func<Slot, bool>> filter = s =>
            s.Date >= DateOnly.FromDateTime(slotModel.StartTime) &&
            s.Date <= DateOnly.FromDateTime(slotModel.EndTime) &&
            (slotModel.Shifts == null || slotModel.Shifts.Contains(s.Shift)) &&
            (slotModel.SlotStatuses == null || slotModel.SlotStatuses.Contains(s.Status)) &&
            (string.IsNullOrEmpty(slotModel.InstructorFirebaseId) ||
             s.Class.InstructorId == slotModel.InstructorFirebaseId) &&
            (string.IsNullOrEmpty(slotModel.StudentFirebaseId) ||
             s.SlotStudents.Any(ss => ss.StudentFirebaseId == slotModel.StudentFirebaseId)) &&
            (string.IsNullOrEmpty(userFirebaseId) || s.Class.InstructorId == userFirebaseId ||
             s.SlotStudents.Any(ss => ss.StudentFirebaseId == userFirebaseId));

        var result = await _unitOfWork.SlotRepository.FindProjectedAsync<SlotDetailModel>(
            filter
        );

        return result;
    }
}