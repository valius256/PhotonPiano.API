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
using PhotonPiano.Shared.Enums;
using PhotonPiano.Shared.Exceptions;
using PhotonPiano.Shared.Utils;

namespace PhotonPiano.BusinessLogic.Services;

public class SlotService : ISlotService
{
    private const int DefaultCacheDurationMinutes = 1440; // Cache 1 ngày (24 giờ)
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

    public async Task<SlotDetailModel> GetSlotDetailById(Guid id, AccountModel? accountModel = default)
    {
        var result = await _unitOfWork.SlotRepository.FindSingleProjectedAsync<SlotDetailModel>(
            s => s.Id == id,
            false
        );

        if (result == null) throw new NotFoundException("Slot not found");

        // Get the slot to access its ClassId and Date
        var slot = await _unitOfWork.SlotRepository.FindSingleAsync(x => x.Id == id);

        // Get total number of slots for this class
        var totalSlots = await _unitOfWork.SlotRepository.CountAsync(s => s.ClassId == slot.ClassId);

        // Calculate the slot number by counting slots up to this date
        var slotNo = await _unitOfWork.SlotRepository.CountAsync(s => s.ClassId == slot.ClassId && s.Date <= slot.Date
        );

        // Assign the calculated values
        result.SlotNo = slotNo;
        result.SlotTotal = totalSlots;

        return result;
    }

    public async Task<List<SlotDetailModel>> GetSlots(GetSlotModel slotModel, AccountModel? accountModel = default)
    {
        Expression<Func<Slot, bool>> filter = s =>
            s.Date >= slotModel.StartTime &&
            s.Date <= slotModel.EndTime &&
            slotModel.Shifts.Contains(s.Shift) &&
            slotModel.SlotStatuses.Contains(s.Status);

        if (accountModel is { Role: Role.Instructor })
            filter = filter.AndAlso(s => s.Class.InstructorId == accountModel.AccountFirebaseId);
        else if (accountModel is { Role: Role.Student })
            filter = filter.AndAlso(s =>
                s.SlotStudents.Any(ss => ss.StudentFirebaseId == accountModel.AccountFirebaseId));

        var result = await _unitOfWork.SlotRepository.FindProjectedAsync<SlotDetailModel>(filter);

        return result;
    }


    public async Task<List<SlotDetailModel>> GetWeeklySchedule(GetSlotModel slotModel, AccountModel accountModel)
    {
        if (slotModel.StartTime > slotModel.EndTime)
            throw new BadRequestException("Start time must be less than end time");

        var classIds = await GetClassIdsForUser(accountModel, slotModel);
        var result = new List<SlotDetailModel>();

        foreach (var classId in classIds)
        {
            var cacheKey = GenerateCacheKey(classId, slotModel.StartTime, accountModel.Role);

            var slots = await _serviceFactory.RedisCacheService.GetAsync<List<SlotDetailModel>>(cacheKey);
            if (slots == null)
            {
                slots = await _unitOfWork.SlotRepository.FindProjectedAsync<SlotDetailModel>(s =>
                    s.ClassId == classId &&
                    s.Date >= slotModel.StartTime &&
                    s.Date <= slotModel.EndTime
                );

                // Lấy tổng số slot của lớp
                var totalSlots = await _unitOfWork.SlotRepository.CountAsync(s => s.ClassId == classId);

                // Cập nhật SlotNo và SlotTotal cho mỗi slot
                foreach (var slot in slots)
                {
                    // Lấy số thứ tự của slot trong lớp
                    var slotNo =
                        await _unitOfWork.SlotRepository.CountAsync(s => s.ClassId == classId && s.Date <= slot.Date
                        );

                    slot.SlotNo = slotNo;
                    slot.SlotTotal = totalSlots;
                }

                await _serviceFactory.RedisCacheService.SaveAsync(
                    cacheKey,
                    slots,
                    TimeSpan.FromMinutes(DefaultCacheDurationMinutes)
                );
            }

            result.AddRange(slots);
        }

        result = ApplyAdditionalFilters(result, slotModel);
        return result;
    }

    public async Task<List<StudentAttendanceModel>> GetAttendanceStatus(Guid slotId)
    {
        var slot = await _unitOfWork.SlotRepository.FindSingleProjectedAsync<Slot>(s => s.Id == slotId);
        if (slot is null) throw new NotFoundException("Slot not found");

        var attendanceStatuses = slot.SlotStudents.Select(ss => new StudentAttendanceModel
        {
            AttendanceStatus = ss.AttendanceStatus,
            StudentFirebaseId = ss.StudentFirebaseId
        }).ToList();
        return attendanceStatuses;
    }

    public async Task CronAutoChangeSlotStatus()
    {
        var vietnamNow = GetVietnamNow();
        var currentDate = DateOnly.FromDateTime(vietnamNow);
        var currentTime = TimeOnly.FromDateTime(vietnamNow);

        // Tập hợp để lưu các ClassId và tuần bị ảnh hưởng
        var affectedClassesAndWeeks = new HashSet<(Guid ClassId, DateOnly StartOfWeek)>();

        var pastSlots =
            await _unitOfWork.SlotRepository.FindAsync(s => s.Date < currentDate && s.Status != SlotStatus.Finished
            );

        foreach (var slot in pastSlots)
        {
            slot.Status = SlotStatus.Finished;
            affectedClassesAndWeeks.Add((slot.ClassId, GetStartOfWeek(slot.Date)));
        }

        var todaySlots = await _unitOfWork.SlotRepository.FindAsync(s =>
            s.Date == currentDate && s.Status != SlotStatus.Finished && s.Status != SlotStatus.Cancelled
        );

        if (!todaySlots.Any())
        {
            if (pastSlots.Any())
            {
                await _unitOfWork.SlotRepository.UpdateRangeAsync(pastSlots);
                await _unitOfWork.SaveChangesAsync();
            }

            return;
        }

        var classIds = todaySlots.Select(s => s.ClassId).Distinct().ToList();

        var affectedClasses = await _unitOfWork.ClassRepository.FindAsync(c => classIds.Contains(c.Id));

        var allRelatedSlots = await _unitOfWork.SlotRepository.FindAsync(s => classIds.Contains(s.ClassId));

        var firstLastSlotMap = allRelatedSlots
            .GroupBy(s => s.ClassId)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    FirstSlot = g.OrderBy(s => s.Date).ThenBy(s => s.Shift).First(),
                    LastSlot = g.OrderByDescending(s => s.Date).ThenByDescending(s => s.Shift).First()
                });

        var classStatusMap = affectedClasses.ToDictionary(c => c.Id, c => c);
        var classesToUpdate = new List<Class>();

        foreach (var slot in todaySlots)
        {
            var shiftStart = GetShiftStartTime(slot.Shift);
            var shiftEnd = GetShiftEndTime(slot.Shift);

            if (currentTime > shiftEnd && slot.Status != SlotStatus.Finished)
            {
                slot.Status = SlotStatus.Finished;
            }
            else if (currentTime >= shiftStart && slot.Status != SlotStatus.Ongoing)
            {
                slot.Status = SlotStatus.Ongoing;
            }
        }

        var firstSlots = firstLastSlotMap.Keys.Select(c => firstLastSlotMap[c].FirstSlot).ToList();

        foreach (var firstSlot in firstSlots)
        {
            var shiftStart = GetShiftStartTime(firstSlot.Shift);
            if (currentTime >= shiftStart && firstSlot.Status != SlotStatus.Ongoing)
            {
                if (classStatusMap.TryGetValue(firstSlot.ClassId, out var cls) &&
                    cls.Status != ClassStatus.Ongoing)
                {
                    cls.Status = ClassStatus.Ongoing;
                    classesToUpdate.Add(cls);
                }
            }
        }

        var laSlots = firstLastSlotMap.Keys.Select(c => firstLastSlotMap[c].LastSlot).ToList();

        foreach (var lastSlot in laSlots)
        {
            var shiftEnd = GetShiftEndTime(lastSlot.Shift);
            if (currentTime >= shiftEnd && lastSlot.Status != SlotStatus.Finished)
            {
                if (classStatusMap.TryGetValue(lastSlot.ClassId, out var cls) &&
                    cls.Status != ClassStatus.Finished)
                {
                    cls.Status = ClassStatus.Finished;
                    classesToUpdate.Add(cls);
                }
            }    
        }

        // if (pastSlots.Count > 0)
        // {
        //     foreach (var slot in pastSlots)
        //     {
        //         var shiftStart = GetShiftStartTime(slot.Shift);
        //         var shiftEnd = GetShiftEndTime(slot.Shift);
        //         var classId = slot.ClassId;
        //         var slotId = slot.Id;
        //         var startOfWeek = GetStartOfWeek(slot.Date);
        //
        //         if (currentTime > shiftEnd && slot.Status != SlotStatus.Finished)
        //         {
        //             affectedClassesAndWeeks.Add((classId, startOfWeek));
        //
        //             if (firstLastSlotMap[classId].LastSlot.Id == slotId &&
        //                 classStatusMap.TryGetValue(classId, out var cls) &&
        //                 cls.Status != ClassStatus.Finished)
        //             {
        //                 cls.Status = ClassStatus.Finished;
        //                 classesToUpdate.Add(cls);
        //             }
        //         }
        //         else if (currentTime >= shiftStart && slot.Status != SlotStatus.Ongoing)
        //         {
        //             affectedClassesAndWeeks.Add((classId, startOfWeek));
        //
        //             if (firstLastSlotMap[classId].FirstSlot.Id == slotId &&
        //                 classStatusMap.TryGetValue(classId, out var cls) &&
        //                 cls.Status != ClassStatus.Ongoing)
        //             {
        //                 cls.Status = ClassStatus.Ongoing;
        //                 classesToUpdate.Add(cls);
        //
        //                 // var classDetail = await _serviceFactory.ClassService.GetClassDetailById(classId);
        //                 // await _serviceFactory.TuitionService.CreateTuitionWhenRegisterClass(classDetail);
        //             }
        //         }    
        //     }
        // }
        await _unitOfWork.ClassRepository.UpdateRangeAsync(classesToUpdate);
        await _unitOfWork.SlotRepository.UpdateRangeAsync(pastSlots.Concat(todaySlots).ToList());
        await _unitOfWork.SaveChangesAsync();

        foreach (var (classId, startOfWeek) in affectedClassesAndWeeks)
            await InvalidateCacheForClassAsync(classId, startOfWeek);
    }

    public async Task<SlotModel> CreateSlot(CreateSlotModel createSlotModel, string accountFirebaseId)
    {
        var slotStartTime = GetShiftStartTime(createSlotModel.Shift);
        var slotStartDate = new DateTime(createSlotModel.Date.Year, createSlotModel.Date.Month,
            createSlotModel.Date.Day, slotStartTime.Hour, slotStartTime.Minute, 0);
        if (slotStartDate <= DateTime.UtcNow.AddHours(7))
            throw new BadRequestException("Can not add slots in the past");

        var room = await _unitOfWork.RoomRepository.GetByIdAsync(createSlotModel.RoomId);
        if (room is null) throw new NotFoundException("Room not found");

        var classDetail = await _unitOfWork.ClassRepository.Entities.Include(c => c.StudentClasses)
            .FirstOrDefaultAsync(c => c.Id == createSlotModel.ClassId);

        if (classDetail is null) throw new NotFoundException("Class not found");

        var level = await _unitOfWork.LevelRepository.FindSingleAsync(l => l.Id == classDetail.LevelId);
        if (level is null) throw new NotFoundException("Level not found");
        var slotCount = await _unitOfWork.SlotRepository.CountAsync(c => c.ClassId == classDetail.Id);
        if (slotCount >= level.TotalSlots) throw new BadRequestException("This class has enough slots already!");

        if (await IsConflict(createSlotModel.Shift, createSlotModel.Date, createSlotModel.RoomId))
            throw new ConflictException("Can not add slot because of a schedule conflict");

        var dayOffs = await _unitOfWork.DayOffRepository.FindAsync(d => d.EndTime >= DateTime.UtcNow.AddHours(7));
        if (dayOffs.Any(dayOff =>
                dayOff.StartTime <= slotStartDate &&
                dayOff.EndTime >= slotStartDate))
            throw new ConflictException("Can not add slot in day offs period");

        var slot = createSlotModel.Adapt<Slot>();
        slot.TeacherId = classDetail.InstructorId;

        var result = await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var addedSlot = await _unitOfWork.SlotRepository.AddAsync(slot);

            //Add student slots
            var studentSlots = classDetail.StudentClasses.Select(sc => new SlotStudent
            {
                CreatedById = accountFirebaseId,
                SlotId = addedSlot.Id,
                StudentFirebaseId = sc.StudentFirebaseId
            });
            await _unitOfWork.SlotStudentRepository.AddRangeAsync(studentSlots);
            await _unitOfWork.SaveChangesAsync();
            return addedSlot;
        });
        await _serviceFactory.ClassService.UpdateClassStartTime(slot.ClassId);

        //Notification
        if (classDetail.IsPublic)
        {
            var notiMessage =
                $"A new slot has been added to your class ${classDetail.Name}. Day {slot.Date}, Shift {(int)slot.Shift + 1}, Room : {room.Name}";
            var studentSlots = classDetail.StudentClasses.Select(sc => new SlotStudent
            {
                CreatedById = accountFirebaseId,
                SlotId = result.Id,
                StudentFirebaseId = sc.StudentFirebaseId
            });
            await _serviceFactory.NotificationService.SendNotificationToManyAsync(
                studentSlots.Select(ss => ss.StudentFirebaseId).ToList(),
                notiMessage, "");
            if (classDetail.InstructorId != null)
                await _serviceFactory.NotificationService.SendNotificationAsync(classDetail.InstructorId, notiMessage,
                    "");
        }

        return result.Adapt<SlotModel>();
    }

    public async Task<SlotModel> UpdateSlot(UpdateSlotModel updateSlotModel, string accountFirebaseId)
    {
        var slot = await _unitOfWork.SlotRepository.GetByIdAsync(updateSlotModel.Id);
        if (slot is null) throw new NotFoundException("Slot not found");

        if (slot.Status != SlotStatus.NotStarted)
            throw new BadRequestException("Can only update slot that is not started yet!");
        var slotStartTime = GetShiftStartTime(updateSlotModel.Shift ?? slot.Shift);
        var slotStartDate = new DateTime(
            updateSlotModel.Date?.Year ?? slot.Date.Year,
            updateSlotModel.Date?.Month ?? slot.Date.Month,
            updateSlotModel.Date?.Day ?? slot.Date.Day,
            slotStartTime.Hour,
            slotStartTime.Minute, 0);

        if (slotStartDate <= DateTime.UtcNow.AddHours(7))
            throw new BadRequestException("Can not update slots to the past");

        var notiMessage = "";
        if (updateSlotModel.RoomId != null && updateSlotModel.RoomId != slot.RoomId)
        {
            var oldRoom = slot.RoomId != null ? await _unitOfWork.RoomRepository.GetByIdAsync(slot.RoomId.Value) : null;
            var room = await _unitOfWork.RoomRepository.GetByIdAsync(updateSlotModel.RoomId.Value);
            if (room is null) throw new NotFoundException("Room not found");
            slot.RoomId = updateSlotModel.RoomId;
            notiMessage += $" Update from room {oldRoom?.Name} to {room.Name}.";
        }

        if ((updateSlotModel.Date != null && updateSlotModel.Date != slot.Date) ||
            (updateSlotModel.Shift != null && updateSlotModel.Shift != slot.Shift))
        {
            if (await IsConflict(updateSlotModel.Shift ?? slot.Shift, updateSlotModel.Date ?? slot.Date,
                    updateSlotModel.RoomId ?? slot.RoomId))
                throw new ConflictException("Can not update slots because of a schedule conflict");

            notiMessage += $" Update time from day {slot.Date}, shift {(int)slot.Shift + 1} " +
                           $"to day {updateSlotModel.Date ?? slot.Date}, shift {(int)(updateSlotModel.Shift ?? slot.Shift) + 1}.";
        }


        string? oldTeacherId = null;
        Account? newTeacher = null;
        if (updateSlotModel.TeacherId != null && slot.TeacherId != updateSlotModel.TeacherId)
        {
            var teacher =
                await _unitOfWork.AccountRepository.FindFirstAsync(a =>
                    a.AccountFirebaseId == updateSlotModel.TeacherId);
            if (teacher is null) throw new NotFoundException("Teacher not found");
            var slotOfTeacher =
                await _unitOfWork.SlotRepository.FindAsync(s => s.TeacherId == teacher.AccountFirebaseId, false);
            foreach (var teacherSlot in slotOfTeacher)
                if (teacherSlot.Shift == slot.Shift && teacherSlot.Date == slot.Date)
                    throw new ConflictException("Teacher can not be assigned to this slot due to a schedule conflict");

            oldTeacherId = slot.TeacherId;
            newTeacher = teacher;

            slot.TeacherId = updateSlotModel.TeacherId;
            notiMessage += $" Update substitute teacher {teacher.FullName ?? teacher.UserName}.";
        }

        var dayOffs = await _unitOfWork.DayOffRepository.FindAsync(d => d.EndTime >= DateTime.UtcNow.AddHours(7));
        if (dayOffs.Any(dayOff =>
                dayOff.StartTime <= slotStartDate &&
                dayOff.EndTime >= slotStartDate))
            throw new ConflictException("Can not update slot in day offs period");

        var updatedSlot = updateSlotModel.Adapt(slot);
        updatedSlot.UpdatedAt = DateTime.UtcNow.AddHours(7);

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.SlotRepository.UpdateAsync(updatedSlot);
            await _unitOfWork.SaveChangesAsync();
        });
        await _serviceFactory.ClassService.UpdateClassStartTime(slot.ClassId);


        //Notification

        var classDetail = await _unitOfWork.ClassRepository.Entities.Include(c => c.StudentClasses)
            .FirstOrDefaultAsync(c => c.Id == slot.ClassId);


        if (classDetail != null && classDetail.IsPublic)
        {
            if (updateSlotModel.Reason != null) notiMessage += " Reason: " + updateSlotModel.Reason;
            notiMessage = $"A slot has been updated in your class {classDetail.Name}." + notiMessage;
            var studentSlots = classDetail.StudentClasses.Select(sc => new SlotStudent
            {
                CreatedById = accountFirebaseId,
                SlotId = slot.Id,
                StudentFirebaseId = sc.StudentFirebaseId
            });
            await _serviceFactory.NotificationService.SendNotificationToManyAsync(
                studentSlots.Select(ss => ss.StudentFirebaseId).ToList(),
                notiMessage, "");
            if (classDetail.InstructorId != null)
            {
                if (oldTeacherId == null)
                {
                    await _serviceFactory.NotificationService.SendNotificationAsync(classDetail.InstructorId, "",
                        notiMessage);
                }
                else if (newTeacher != null)
                {
                    await _serviceFactory.NotificationService.SendNotificationAsync(classDetail.InstructorId,
                        "[Notification]", $"Center has assigned you to this class" +
                                          $"day {slot.Date}, shift {(int)slot.Shift + 1} of the class {classDetail.Name}");
                    await _serviceFactory.NotificationService.SendNotificationAsync(oldTeacherId, "[Notification]",
                        $"Center has assigned you to ${newTeacher.FullName ?? newTeacher.UserName} to be a substitue teacher of slot  " +
                        $"day {slot.Date}, shift {(int)slot.Shift + 1} of the class {classDetail.Name}",
                        $"{newTeacher.AvatarUrl}");
                }
            }
        }

        return updatedSlot.Adapt<SlotModel>();
    }

    public async Task DeleteSlot(Guid slotId, string accountFirebaseId)
    {
        var slot = await _unitOfWork.SlotRepository.GetByIdAsync(slotId);
        if (slot is null) throw new NotFoundException("Slot not found");

        if (slot.Status != SlotStatus.NotStarted)
            throw new BadRequestException("Can only delete slots that is not started yet!");
        slot.RecordStatus = RecordStatus.IsDeleted;
        slot.DeletedAt = DateTime.UtcNow.AddHours(7);
        //Delete slot students
        var slotStudents = await _unitOfWork.SlotStudentRepository.FindAsync(ss => ss.SlotId == slotId);
        foreach (var student in slotStudents)
        {
            student.RecordStatus = RecordStatus.IsDeleted;
            student.DeletedAt = DateTime.UtcNow.AddHours(7);
        }

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.SlotStudentRepository.UpdateRangeAsync(slotStudents);
            await _unitOfWork.SlotRepository.UpdateAsync(slot);
            await _unitOfWork.SaveChangesAsync();
        });
        await _serviceFactory.ClassService.UpdateClassStartTime(slot.ClassId);

        //Notification
        var classDetail = await _unitOfWork.ClassRepository.GetByIdAsync(slot.ClassId);

        if (classDetail != null && classDetail.IsPublic)
        {
            var notiMessage =
                $"A slot has been removed from your ${classDetail.Name}. Date {slot.Date}, Shift {(int)slot.Shift + 1}";
            await _serviceFactory.NotificationService.SendNotificationToManyAsync(
                slotStudents.Select(ss => ss.StudentFirebaseId).ToList(),
                notiMessage, "");
            if (classDetail.InstructorId != null)
                await _serviceFactory.NotificationService.SendNotificationAsync(classDetail.InstructorId, notiMessage,
                    "");
        }
    }

    public async Task<List<BlankSlotModel>> GetAllBlankSlotInWeeks(DateOnly startDate, DateOnly endDate)
    {
        // Get all activems
        var rooms = await _unitOfWork.RoomRepository.FindAsync(r => r.Status == RoomStatus.Opened);

        // Get all existing slots with rooms (without date filtering)
        var existingSlots = await _unitOfWork.SlotRepository.FindAsync(s => s.RoomId != null);

        // Filter in memory to avoid PostgreSQL date conversion issues
        var filteredSlots = existingSlots.Where(s =>
            s.Date >= startDate &&
            s.Date <= endDate).ToList();

        var blankSlots = new List<BlankSlotModel>();
        const int maxResults = 50;

        // Check every combination of date, shift, and room
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
            foreach (Shift shift in Enum.GetValues(typeof(Shift)))
            {
                // Use current time without subtracting a day, to include yesterday's slots
                var currentDateTime = DateTime.UtcNow.AddHours(7);
                var shiftStartTime = GetShiftStartTime(shift);
                var shiftDateTime = new DateTime(date.Year, date.Month, date.Day,
                    shiftStartTime.Hour, shiftStartTime.Minute, 0);

                // Only skip slots that are before current time
                if (shiftDateTime < currentDateTime)
                    continue;

                foreach (var room in rooms)
                {
                    // Check if this slot is already taken (using in-memory filtered collection)
                    var slotExists = filteredSlots.Any(s =>
                        s.Date == date &&
                        s.Shift == shift &&
                        s.RoomId == room.Id);

                    if (!slotExists)
                    {
                        // Check day-offs
                        var slotDateTime = new DateTime(date.Year, date.Month, date.Day,
                            shiftStartTime.Hour, shiftStartTime.Minute, 0, DateTimeKind.Utc);

                        var inDayOff = await _unitOfWork.DayOffRepository.AnyAsync(d =>
                            d.StartTime <= slotDateTime && d.EndTime >= slotDateTime);

                        if (!inDayOff)
                        {
                            blankSlots.Add(new BlankSlotModel
                            {
                                Date = date,
                                Shift = shift,
                                RoomId = room.Id,
                                RoomName = room.Name
                            });

                            if (blankSlots.Count >= maxResults)
                                return blankSlots;
                        }
                    }
                }
            }

        return blankSlots;
    }

    public async Task<bool> CancelSlot(CancelSlotModel model, string accountFirebaseId)
    {
        var slotInDb = await _unitOfWork.SlotRepository.FindFirstProjectedAsync<Slot>(s => s.Id == model.SlotId, false);

        if (slotInDb == null) throw new NotFoundException("Slot not found");

        if (slotInDb.Status == SlotStatus.Cancelled)
            throw new IllegalArgumentException("Slot has already been cancelled");

        if (slotInDb.Status == SlotStatus.Finished)
            throw new IllegalArgumentException("Can not cancel a finished slot");

        if (slotInDb.Status == SlotStatus.Ongoing) throw new IllegalArgumentException("Can not cancel an ongoing slot");

        // mark slot to be canceled 
        slotInDb.Status = SlotStatus.Cancelled;
        slotInDb.SlotNote = model.CancelReason;
        slotInDb.UpdatedAt = DateTime.UtcNow.AddHours(7);
        slotInDb.CancelById = accountFirebaseId;


        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.SlotRepository.UpdateAsync(slotInDb);
        });

        // removed all slotstudent of that slo
        await _unitOfWork.SlotStudentRepository.ExecuteDeleteAsync(ss => ss.SlotId == model.SlotId);

        // removed from redis 
        await InvalidateCacheForClassAsync(slotInDb.ClassId, slotInDb.Date);
        return true;
    }

    public async Task<SlotDetailModel> PublicNewSlot(PublicNewSlotModel model, string accountFirebaseId)
    {
        var roomInDb = await _serviceFactory.RoomService.GetRoomDetailById(model.RoomId);

        if (roomInDb.Status == RoomStatus.Closed) throw new IllegalArgumentException("Room is closed");

        var isConflict = await _unitOfWork.SlotRepository.AnyAsync(s =>
            s.RoomId == model.RoomId && s.Shift == model.Shift && s.Date == model.Date);

        if (isConflict) throw new IllegalArgumentException("Room is already open for another class");

        // new slot in same class 
        var newSlot = new Slot
        {
            Id = Guid.NewGuid(),
            ClassId = model.ClassId,
            RoomId = model.RoomId,
            Shift = model.Shift,
            Date = model.Date,
            Status = SlotStatus.NotStarted
        };

        var studentClasses = await _unitOfWork.StudentClassRepository.FindAsync(sc => sc.ClassId == model.ClassId);
        var studentIds = studentClasses.Select(x => x.StudentFirebaseId).ToList();

        var numOfStudent = studentIds.Count;

        var slotStudents = new List<SlotStudent>();

        foreach (var studentId in studentIds)
            slotStudents.Add(new SlotStudent
            {
                SlotId = newSlot.Id,
                StudentFirebaseId = studentId,
                CreatedById = accountFirebaseId
            });

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.SlotRepository.AddAsync(newSlot);

            if (numOfStudent != slotStudents.Count)
                throw new IllegalArgumentException("Number of student students are not equal, check again");
            await _unitOfWork.SlotStudentRepository.AddRangeAsync(slotStudents);
        });


        var classInDb = await _serviceFactory.ClassService.GetClassDetailById(newSlot.ClassId);
        //Notification
        await _serviceFactory.NotificationService.SendNotificationToManyAsync(
            studentIds,
            $"Class {classInDb.Name} will have a new slot in {newSlot.Date}, at room {roomInDb.Name}.",
            ""
        );

        // await InvalidateCacheForClassAsync(newSlot.ClassId, newSlot.Date);

        await _serviceFactory.RedisCacheService.DeleteByPatternAsync("schedule:*");
        return newSlot.Adapt<SlotDetailModel>();
    }

    public async Task<List<AccountSimpleModel>> GetAllTeacherCanBeAssignedToSlot(Guid slotId, string accountFirebaseId)
    {
        var currentSlot = await _unitOfWork.SlotRepository.FindSingleProjectedAsync<Slot>(x => x.Id == slotId, false);
        if (currentSlot == null) throw new NotFoundException("Slot not found");

        var allTeachers = await _unitOfWork.AccountRepository.FindAsync(a =>
            a.Role == Role.Instructor &&
            a.Status == AccountStatus.Active);

        var slotsAtSameTime = await _unitOfWork.SlotRepository.FindAsync(s =>
            s.Id != slotId &&
            s.Date == currentSlot.Date &&
            s.Shift == currentSlot.Shift &&
            s.Status != SlotStatus.Cancelled);

        var classIdsWithConflictingSlots = slotsAtSameTime.Select(s => s.ClassId).Distinct().ToList();
        var classesWithTeachers = await _unitOfWork.ClassRepository.FindAsync(c =>
            classIdsWithConflictingSlots.Contains(c.Id));

        var busyTeacherIds = classesWithTeachers
            .Where(c => c.InstructorId != null)
            .Select(c => c.InstructorId)
            .Distinct()
            .ToList();

        // Also exclude the instructor currently assigned to the slot 
        if (currentSlot.Class?.InstructorId != null) busyTeacherIds.Add(currentSlot.Class.InstructorId);

        // Filter available teachers
        var availableTeachers = allTeachers
            .Where(t => !busyTeacherIds.Contains(t.AccountFirebaseId))
            .ToList();

        if (!availableTeachers.Any()) throw new NotFoundException("No available teachers found for this slot.");

        return availableTeachers.Adapt<List<AccountSimpleModel>>();
    }

    public async Task<SlotDetailModel> AssignTeacherToSlot(Guid slotId, string teacherFirebaseId, string Reason,
        string staffAccountFirebaseId)
    {
        var teacher =
            await _unitOfWork.AccountRepository.FindSingleAsync(
                a => a.AccountFirebaseId == teacherFirebaseId && a.Role == Role.Instructor, false);

        if (teacher is null) throw new NotFoundException("Teacher not found");

        var slot = await _unitOfWork.SlotRepository.FindSingleAsync(s => s.Id == slotId, false);

        if (slot == null) throw new NotFoundException("Slot not found");

        if (slot.Status != SlotStatus.NotStarted)
            throw new BadRequestException("Can only assign teacher to slots that are not started yet!");

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            slot.TeacherId = teacherFirebaseId;
            slot.UpdatedAt = DateTime.UtcNow.AddHours(7);
            slot.UpdateById = staffAccountFirebaseId;
            slot.SlotNote = Reason;

            await _unitOfWork.SlotRepository.UpdateAsync(slot);
            await _unitOfWork.SaveChangesAsync();
        });

        await _serviceFactory.RedisCacheService.DeleteByPatternAsync("schedule:*");

        return await GetSlotDetailById(slotId);
    }

    public async Task<List<StudentAttendanceResult>> GetAllAttendanceResultByClassId(Guid classId)
    {
        var systemConfig = await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.MaxAbsenceRate);
        if (systemConfig == null) throw new NotFoundException("System config not found");

        decimal.TryParse(systemConfig.ConfigValue, out var attendanceThreshold);

        var currentClass = await _serviceFactory.ClassService.GetClassDetailById(classId);

        if (currentClass == null) throw new NotFoundException("Class not found");

        var studentAttendanceResults = new List<StudentAttendanceResult>();

        var studentClasses = await _unitOfWork.StudentClassRepository.FindAsync(sc => sc.ClassId == classId);

        foreach (var studentClass in studentClasses)
        {
            var studentAttendanceResult = new StudentAttendanceResult
            {
                StudentId = studentClass.StudentFirebaseId,
                AttendancePercentage = 0,
                TotalSlots = 0,
                AttendedSlots = 0,
                IsPassed = false
            };

            var studentSlots =
                await _unitOfWork.SlotStudentRepository.FindAsync(ss =>
                    ss.StudentFirebaseId == studentClass.StudentFirebaseId);
            var totalSlots = studentSlots.Count;
            var attendedSlots = studentSlots.Count(ss => ss.AttendanceStatus == AttendanceStatus.Attended);

            if (totalSlots > 0)
                studentAttendanceResult.AttendancePercentage = (decimal)attendedSlots / totalSlots * 100;

            studentAttendanceResult.TotalSlots = totalSlots;
            studentAttendanceResult.AttendedSlots = attendedSlots;

            if (studentAttendanceResult.AttendancePercentage < attendanceThreshold)
            {
                studentAttendanceResult.IsPassed = true;
                studentAttendanceResults.Add(studentAttendanceResult);
            }
        }

        return studentAttendanceResults;
    }

    private async Task<List<Guid>> GetClassIdsForUser(AccountModel accountModel, GetSlotModel slotModel)
    {
        Expression<Func<Slot, bool>> filter = s =>
            s.Date >= slotModel.StartTime && s.Date <= slotModel.EndTime;

        filter = accountModel.Role switch
        {
            Role.Instructor => filter.AndAlso(s => s.Class.InstructorId == accountModel.AccountFirebaseId),
            Role.Student => filter.AndAlso(s =>
                s.SlotStudents.Any(ss => ss.StudentFirebaseId == accountModel.AccountFirebaseId)),
            _ => filter
        };

        if (slotModel.InstructorFirebaseIds.Any())
            filter = filter.AndAlso(s =>
                s.Class.InstructorId != null && slotModel.InstructorFirebaseIds.Contains(s.Class.InstructorId));

        if (slotModel.ClassIds.Any())
            filter = filter.AndAlso(s => slotModel.ClassIds.Contains(s.ClassId));

        // Truy vấn danh sách ClassId duy nhất
        var slots = await _unitOfWork.SlotRepository.FindAsync(filter);
        return slots.Select(s => s.ClassId).Distinct().ToList();
    }

    private List<SlotDetailModel> ApplyAdditionalFilters(List<SlotDetailModel> slots, GetSlotModel slotModel)
    {
        if (slotModel.Shifts.Any())
            slots = slots.Where(s => slotModel.Shifts.Contains(s.Shift)).ToList();

        if (slotModel.SlotStatuses.Any())
            slots = slots.Where(s => slotModel.SlotStatuses.Contains(s.Status)).ToList();

        return slots;
    }

    private DateOnly GetStartOfWeek(DateOnly date)
    {
        var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-1 * diff);
    }

    private string GenerateCacheKey(Guid classId, DateOnly startOfWeek, Role role)
    {
        return $"schedule:{role}:class:{classId}:week:{startOfWeek:yyyyMMdd}";
    }


    public async Task InvalidateCacheForClassAsync(Guid classId, DateOnly slotDate)
    {
        var startOfWeek = GetStartOfWeek(slotDate);
        foreach (Role role in Enum.GetValues(typeof(Role)))
        {
            var cacheKey = GenerateCacheKey(classId, startOfWeek, role);
            await _serviceFactory.RedisCacheService.DeleteAsync(cacheKey);
        }
    }

    private async Task<bool> IsConflict(Shift shift, DateOnly date, Guid? roomId)
    {
        if (roomId is null) return false;
        return await _unitOfWork.SlotRepository.AnyAsync(s => s.RoomId == roomId && s.Shift == shift && s.Date == date)
               || await _unitOfWork.EntranceTestRepository.AnyAsync(s =>
                   s.RoomId == roomId && s.Shift == shift && s.Date == date);
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

    protected virtual DateTime GetVietnamNow()
    {
        return DateTime.UtcNow.AddHours(7);
    }
}