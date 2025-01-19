using System.Linq.Expressions;
using PhotonPiano.BusinessLogic.BusinessModel.Slot;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;

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