using System.Linq.Expressions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Slot;
using PhotonPiano.BusinessLogic.Extensions;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Exceptions;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
    
    public TimeOnly GetShiftEndTime(Shift shift)
    {
        var shiftStartTimes = new Dictionary<Shift, TimeOnly>
        {
            { Shift.Shift1_7h_8h30, new TimeOnly(8, 30) },
            { Shift.Shift2_8h45_10h15, new TimeOnly(10, 45) },
            { Shift.Shift3_10h45_12h, new TimeOnly(12, 00) },
            { Shift.Shift4_12h30_14h00, new TimeOnly(14, 00) },
            { Shift.Shift5_14h15_15h45, new TimeOnly(15, 45) },
            { Shift.Shift6_16h00_17h30, new TimeOnly(17, 30) },
            { Shift.Shift7_18h_19h30, new TimeOnly(19, 30) },
            { Shift.Shift8_19h45_21h15, new TimeOnly(21, 15) }
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
        var cacheKey =
            $"GetWeeklyScheduleAsync:{slotModel.StartTime}:{slotModel.EndTime}:{accountModel.AccountFirebaseId}";

        if (accountModel.Role != Role.Staff && slotModel.IsFilterEmpty())
        {
           
            var cachedResult = await _serviceFactory.RedisCacheService.GetAsync<List<SlotSimpleModel>>(cacheKey);
            if (cachedResult != null) return cachedResult;
        }

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

         await _serviceFactory.RedisCacheService.SaveAsync(cacheKey, result, TimeSpan.FromDays(7));

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

    public async Task CronJobAutoChangeSlotStatus()
    {
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));
        var currentTime = TimeOnly.FromDateTime(DateTime.UtcNow.AddHours(7));

        var allSlotsForToday = await _unitOfWork.SlotRepository.FindAsync(s => s.Date == currentDate);

        foreach (var slot in allSlotsForToday)
        {
            var shiftStartTime = GetShiftStartTime(slot.Shift);
            var shiftEndTime = GetShiftEndTime(slot.Shift);
            if (currentTime >=  shiftStartTime)
            {
                slot.Status = SlotStatus.Ongoing;
            }
            if (currentTime > shiftEndTime)
            {
                slot.Status = SlotStatus.Finished;
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<SlotModel> CreateSlot(CreateSlotModel createSlotModel, string accountFirebaseId)
    {
        var slotStartTime = GetShiftStartTime(createSlotModel.Shift);
        var slotStartDate = new DateTime(createSlotModel.Date.Year,createSlotModel.Date.Month,createSlotModel.Date.Day,slotStartTime.Hour,slotStartTime.Minute,0);
        if (slotStartDate <= DateTime.UtcNow.AddHours(7))
        {
            throw new BadRequestException("Can not add slots in the past");
        }

        var room = await _unitOfWork.RoomRepository.GetByIdAsync(createSlotModel.RoomId);
        if (room is null)
        {
            throw new NotFoundException("Room not found");
        }

        var classInfo = await _unitOfWork.ClassRepository.GetByIdAsync(createSlotModel.ClassId);
        if (classInfo is null)
        {
            throw new NotFoundException("Class not found");
        }

        var config = await _serviceFactory.SystemConfigService.GetSystemConfigValueBaseOnLevel((int)classInfo.Level + 1);
        var slotCount = await _unitOfWork.SlotRepository.CountAsync(c => c.ClassId == classInfo.Id);
        if (slotCount >= config.TotalSlot)
        {
            throw new BadRequestException("This class has enough slots already!");
        }

        if (await IsConflict(createSlotModel.Shift, createSlotModel.Date, createSlotModel.RoomId))
        {
            throw new ConflictException("Can not add slot because of a schedule conflict");
        }
        var dayOffs = await _unitOfWork.DayOffRepository.FindAsync(d => d.EndTime >= DateTime.UtcNow.AddHours(7));
        if(dayOffs.Any(dayOff =>
                dayOff.StartTime <= slotStartDate &&
                dayOff.EndTime >= slotStartDate))
        {
            throw new ConflictException("Can not add slot in day offs period");
        }


        var slot = createSlotModel.Adapt<Slot>();
        var addedSlot = await _unitOfWork.SlotRepository.AddAsync(slot);

        await _unitOfWork.SaveChangesAsync();

        return addedSlot.Adapt<SlotModel>();
    }

    private async Task<bool> IsConflict(Shift shift, DateOnly date, Guid? roomId)
    {
        if (roomId is null)
        {
            return false;
        }
        return await _unitOfWork.SlotRepository.AnyAsync(s => s.RoomId == roomId && s.Shift == shift && s.Date == date);
    }
    public async Task<SlotModel> UpdateSlot(UpdateSlotModel updateSlotModel, string accountFirebaseId)
    {
        var slot = await _unitOfWork.SlotRepository.GetByIdAsync(updateSlotModel.Id);
        if (slot is null)
        {
            throw new NotFoundException("Slot not found");
        }

        if (slot.Status != SlotStatus.NotStarted)
        {
            throw new BadRequestException("Can only update slot that is not started yet!");
        }

        var slotStartTime = GetShiftStartTime(updateSlotModel.Shift ?? slot.Shift);
        var slotStartDate = new DateTime(
            updateSlotModel.Date?.Year ?? slot.Date.Year, 
            updateSlotModel.Date?.Month ?? slot.Date.Month, 
            updateSlotModel.Date?.Day ?? slot.Date.Day, 
            slotStartTime.Hour,
            slotStartTime.Minute, 0);

        if (slotStartDate <= DateTime.UtcNow.AddHours(7))
        {
            throw new BadRequestException("Can not update slots to the past");
        }
        if (updateSlotModel.RoomId != null)
        {
            var room = await _unitOfWork.RoomRepository.GetByIdAsync(updateSlotModel.RoomId.Value);
            if (room is null)
            {
                throw new NotFoundException("Room not found");
            }
            slot.RoomId = updateSlotModel.RoomId;
        }

        if (await IsConflict(updateSlotModel.Shift ?? slot.Shift, updateSlotModel.Date ?? slot.Date, updateSlotModel.RoomId ?? slot.RoomId))
        {
            throw new ConflictException("Can not update slots because of a schedule conflict");
        }

        var dayOffs = await _unitOfWork.DayOffRepository.FindAsync(d => d.EndTime >= DateTime.UtcNow.AddHours(7));
        if (dayOffs.Any(dayOff =>
                dayOff.StartTime <= slotStartDate &&
                dayOff.EndTime >= slotStartDate))
        {
            throw new ConflictException("Can not update slot in day offs period");
        }

        var updatedSlot = updateSlotModel.Adapt(slot);
        updatedSlot.UpdatedAt = DateTime.UtcNow.AddHours(7);

        await _unitOfWork.SlotRepository.UpdateAsync(updatedSlot);
        await _unitOfWork.SaveChangesAsync();

        return updatedSlot.Adapt<SlotModel>();
    }

    public async Task DeleteSlot(Guid slotId, string accountFirebaseId)
    {
        var slot = await _unitOfWork.SlotRepository.GetByIdAsync(slotId);
        if (slot is null)
        {
            throw new NotFoundException("Slot not found");
        }

        if (slot.Status != SlotStatus.NotStarted)
        {
            throw new BadRequestException("Can only delete slots that is not started yet!");
        }

        slot.RecordStatus = RecordStatus.IsDeleted;
        slot.DeletedAt = DateTime.UtcNow.AddHours(7);

        await _unitOfWork.SlotRepository.UpdateAsync(slot);
        await _unitOfWork.SaveChangesAsync();
    }
}