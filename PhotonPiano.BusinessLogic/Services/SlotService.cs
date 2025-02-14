using System.Linq.Expressions;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Slot;
using PhotonPiano.BusinessLogic.Extensions;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services;

public class SlotService : ISlotService
{
    private readonly IServiceFactory _serviceFactory;
    private readonly IUnitOfWork _unitOfWork;

    public SlotService(IServiceFactory serviceFactory, IUnitOfWork unitOfWork)
    {
        _serviceFactory = serviceFactory;
        _unitOfWork = unitOfWork;
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
            throw new InvalidOperationException("Invalid shift value.");

        return shiftStartTime;
    }

    public async Task<SlotDetailModel> GetSLotDetailById(Guid id, AccountModel? accountModel = default)
    {
        var result = await _unitOfWork.SlotRepository.FindSingleProjectedAsync<SlotDetailModel>(
            s => s.Id == id,
            false
        );


        if (result == null) throw new NotFoundException("Slot not found");

        return result;
    }

    public async Task<List<SlotDetailModel>> GetSlotsAsync(GetSlotModel slotModel, AccountModel? accountModel = default)
    {
        Expression<Func<Slot, bool>> filter = s =>
            s.Date >= slotModel.StartTime &&
            s.Date <= slotModel.EndTime &&
            (slotModel.Shifts == null || slotModel.Shifts.Contains(s.Shift)) &&
            (slotModel.SlotStatuses == null || slotModel.SlotStatuses.Contains(s.Status));

        if (accountModel is { Role: Role.Instructor })
            filter = filter.AndAlso(s => s.Class.InstructorId == accountModel.AccountFirebaseId);
        else if (accountModel is { Role: Role.Student })
            filter = filter.AndAlso(s =>
                s.SlotStudents.Any(ss => ss.StudentFirebaseId == accountModel.AccountFirebaseId));

        var result = await _unitOfWork.SlotRepository.FindProjectedAsync<SlotDetailModel>(filter);

        return result;
    }


    public async Task<List<SlotSimpleModel>> GetWeeklyScheduleAsync(GetSlotModel slotModel, AccountModel accountModel)
    {
        // var instructorIds = slotModel.InstructorFirebaseIds is { Count: > 0 }
        //     ? string.Join(',', slotModel.InstructorFirebaseIds)
        //     : "All";
        //
        // var shifts = slotModel.Shifts is { Count: > 0 }
        //     ? string.Join(',', slotModel.Shifts)
        //     : "All";
        //
        // var slotStatuses = slotModel.SlotStatuses is { Count: > 0 }
        //     ? string.Join(',', slotModel.SlotStatuses)
        //     : "All";
        //
        // var classIds = slotModel.ClassIds is { Count: > 0 }
        //     ? string.Join(',', slotModel.ClassIds)
        //     : "All";
        //
        // var studentId = !string.IsNullOrEmpty(slotModel.StudentFirebaseId)
        //     ? slotModel.StudentFirebaseId
        //     : "All";
        //
        // var cacheKey =
        //     $"WeeklySchedule:{slotModel.StartTime:yyyyMMdd}:{slotModel.EndTime:yyyyMMdd}:{accountModel.AccountFirebaseId}:" +
        //     $"{instructorIds}:{studentId}:{shifts}:{slotStatuses}:{classIds}";
        //
        //
        // // var cacheKey =
        // //     $"GetWeeklyScheduleAsync:{slotModel.StartTime}:{slotModel.EndTime}:{accountModel.AccountFirebaseId}";
        //
        // // Only use cache if no filters are applied
        // if (slotModel.IsFilterEmpty())
        // {
        //     var cachedResult = await _serviceFactory.RedisCacheService.GetAsync<List<SlotSimpleModel>>(cacheKey);
        //     if (cachedResult != null) return cachedResult;
        // }

        Expression<Func<Slot, bool>> filter = s =>
            s.Date >= slotModel.StartTime &&
            s.Date <= slotModel.EndTime;

        // Apply role-based filtering only for non-staff roles
        if (accountModel.Role != Role.Staff)
            switch (accountModel.Role)
            {
                case Role.Instructor:
                    filter = filter.AndAlso(s =>
                        s.Class.InstructorId == accountModel.AccountFirebaseId);
                    break;
                case Role.Student:
                    filter = filter.AndAlso(s =>
                        s.SlotStudents.Any(ss => ss.StudentFirebaseId == accountModel.AccountFirebaseId));
                    break;
            }

        if (!slotModel.IsPropertyNull(nameof(GetSlotModel.InstructorFirebaseIds)))
            filter = filter.AndAlso(s =>
                s.Class.InstructorId != null &&
                slotModel.InstructorFirebaseIds != null && (slotModel.InstructorFirebaseIds.Count == 0 ||
                                                            slotModel.InstructorFirebaseIds.Contains(
                                                                s.Class.InstructorId)));


        if (!slotModel.IsPropertyNull(nameof(GetSlotModel.Shifts)))
            filter = filter.AndAlso(s => slotModel.Shifts.Contains(s.Shift));

        if (!slotModel.IsPropertyNull(nameof(GetSlotModel.SlotStatuses)))
            filter = filter.AndAlso(s => slotModel.SlotStatuses.Contains(s.Status));

        if (!slotModel.IsPropertyNull(nameof(GetSlotModel.ClassIds)))
            filter = filter.AndAlso(s => slotModel.ClassIds.Contains(s.ClassId));

        var result = await _unitOfWork.SlotRepository.FindProjectedAsync<SlotSimpleModel>(filter);

        // todo: if role is staff so no need to cache cause the filter is only applied in role staff 

        // await _serviceFactory.RedisCacheService.SaveAsync(cacheKey, result, TimeSpan.FromDays(7));

        return result;
    }


    public async Task<List<StudentAttendanceModel>> GetAttendanceStatusAsync(Guid slotId)
    {
        var slot = await _unitOfWork.SlotRepository.FindSingleProjectedAsync<Slot>(s => s.Id == slotId);
        if (slot == null) throw new NotFoundException("Slot not found");

        var attendanceStatuses = slot.SlotStudents.Select(ss => new StudentAttendanceModel
        {
            AttendanceStatus = ss.AttendanceStatus,
            StudentFirebaseId = ss.StudentFirebaseId
        }).ToList();
        return attendanceStatuses;
    }
}