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

namespace PhotonPiano.BusinessLogic.Services;

public class SlotService : ISlotService
{
    private readonly IServiceFactory _serviceFactory;
    private readonly IUnitOfWork _unitOfWork;
    private const int DefaultCacheDurationMinutes = 1440; // Cache 1 ngày (24 giờ)
    
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
            (slotModel.Shifts.Contains(s.Shift)) &&
            (slotModel.SlotStatuses.Contains(s.Status));

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
  
        // Tạo danh sách các ClassId cần truy vấn
        List<Guid> classIds = await GetClassIdsForUser(accountModel, slotModel);

        // Tạo danh sách kết quả
        var result = new List<SlotSimpleModel>();

        // Duyệt qua từng ClassId để lấy slot
        foreach (var classId in classIds)
        {
            string cacheKey = GenerateCacheKey(classId, slotModel.StartTime, accountModel.Role);

            // Kiểm tra cache
            var cachedSlots = await _serviceFactory.RedisCacheService.GetAsync<List<SlotSimpleModel>>(cacheKey);
            if (cachedSlots != null)
            {
                result.AddRange(cachedSlots);
                continue;
            }

            // Nếu không có trong cache, truy vấn database
            var slots = await _unitOfWork.SlotRepository.FindProjectedAsync<SlotSimpleModel>(
                s => s.ClassId == classId &&
                     s.Date >= slotModel.StartTime &&
                     s.Date < slotModel.EndTime 
            );

            // Lưu vào cache
            await _serviceFactory.RedisCacheService.SaveAsync(
                cacheKey,
                slots,
                TimeSpan.FromMinutes(DefaultCacheDurationMinutes)
            );

            result.AddRange(slots);
        }

        // Lọc thêm nếu có các bộ lọc khác
        result = ApplyAdditionalFilters(result, slotModel);

        return result;
    }

    private async Task<List<Guid>> GetClassIdsForUser(AccountModel accountModel, GetSlotModel slotModel)
    {
        Expression<Func<Slot, bool>> filter = s =>
            s.Date >= slotModel.StartTime && s.Date <= slotModel.EndTime;

        filter = accountModel.Role switch
        {
            Role.Instructor => filter.AndAlso(s => s.Class.InstructorId == accountModel.AccountFirebaseId),
            Role.Student => filter.AndAlso(s => s.SlotStudents.Any(ss => ss.StudentFirebaseId == accountModel.AccountFirebaseId)),
            _ => filter
        };

        if (slotModel.InstructorFirebaseIds.Any())
            filter = filter.AndAlso(s => s.Class.InstructorId != null && slotModel.InstructorFirebaseIds.Contains(s.Class.InstructorId));

        if (slotModel.ClassIds.Any())
            filter = filter.AndAlso(s => slotModel.ClassIds.Contains(s.ClassId));

        // Truy vấn danh sách ClassId duy nhất
        var slots = await _unitOfWork.SlotRepository.FindAsync(filter);
        return slots.Select(s => s.ClassId).Distinct().ToList();
    }

    private List<SlotSimpleModel> ApplyAdditionalFilters(List<SlotSimpleModel> slots, GetSlotModel slotModel)
    {
        if (slotModel.Shifts.Any())
            slots = slots.Where(s => slotModel.Shifts.Contains(s.Shift)).ToList();

        if (slotModel.SlotStatuses.Any())
            slots = slots.Where(s => slotModel.SlotStatuses.Contains(s.Status)).ToList();

        return slots;
    }

    private DateOnly GetStartOfWeek(DateOnly date)
    {
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-1 * diff);
    }

    private string GenerateCacheKey(Guid classId, DateOnly startOfWeek, Role role)
    {
        return $"schedule:{role}:class:{classId}:week:{startOfWeek:yyyyMMdd}";
    }


    public async Task InvalidateCacheForClassAsync(Guid classId, DateOnly slotDate)
    {
        DateOnly startOfWeek = GetStartOfWeek(slotDate);
        foreach (Role role in Enum.GetValues(typeof(Role)))
        {
            string cacheKey = GenerateCacheKey(classId, startOfWeek, role);
            await _serviceFactory.RedisCacheService.DeleteAsync(cacheKey);
        }
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

  public async Task CronAutoChangeSlotStatus()
    {
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7));
        var currentTime = TimeOnly.FromDateTime(DateTime.UtcNow.AddHours(7));

        // Tập hợp để lưu các ClassId và tuần bị ảnh hưởng
        var affectedClassesAndWeeks = new HashSet<(Guid ClassId, DateOnly StartOfWeek)>();

        //  Cập nhật slot trong quá khứ (chỉ lấy slot chưa Finished)
        var pastSlots = await _unitOfWork.SlotRepository.FindAsync(
            s => s.Date < currentDate && s.Status != SlotStatus.Finished
        );

        foreach (var slot in pastSlots)
        {
            slot.Status = SlotStatus.Finished;
            // Thêm ClassId và tuần của slot vào tập hợp
            affectedClassesAndWeeks.Add((slot.ClassId, GetStartOfWeek(slot.Date)));
        }

        // Cập nhật slot trong ngày hiện tại (chỉ lấy slot có khả năng thay đổi trạng thái)
        var todaySlots = await _unitOfWork.SlotRepository.FindAsync(
            s => s.Date == currentDate && s.Status != SlotStatus.Finished
        );

        foreach (var slot in todaySlots)
        {
            var shiftStartTime = GetShiftStartTime(slot.Shift);
            var shiftEndTime = GetShiftEndTime(slot.Shift);

            if (currentTime > shiftEndTime && slot.Status != SlotStatus.Finished)
            {
                slot.Status = SlotStatus.Finished;
                affectedClassesAndWeeks.Add((slot.ClassId, GetStartOfWeek(slot.Date)));
            }
            else if (currentTime >= shiftStartTime && slot.Status != SlotStatus.Ongoing)
            {
                slot.Status = SlotStatus.Ongoing;
                affectedClassesAndWeeks.Add((slot.ClassId, GetStartOfWeek(slot.Date)));
            }
        }

        //  Lưu thay đổi vào database
        await _unitOfWork.SaveChangesAsync();

        //  Invalidation cache có chọn lọc
        foreach (var (classId, startOfWeek) in affectedClassesAndWeeks)
        {
            await InvalidateCacheForClassAsync(classId, startOfWeek);
        }
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

        var classDetail = await _unitOfWork.ClassRepository.Entities.Include(c => c.StudentClasses)
            .FirstOrDefaultAsync(c => c.Id == createSlotModel.ClassId);

        if (classDetail is null)
        {
            throw new NotFoundException("Class not found");
        }

        var level = await _unitOfWork.LevelRepository.FindSingleAsync(l => l.Id == classDetail.LevelId);
        if (level is null)
        {
            throw new NotFoundException("Level not found");
        }
        var slotCount = await _unitOfWork.SlotRepository.CountAsync(c => c.ClassId == classDetail.Id);
        if (slotCount >= level.TotalSlots)
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

        var result = await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var addedSlot = await _unitOfWork.SlotRepository.AddAsync(slot);

            //Add student slots
            var studentSlots = classDetail.StudentClasses.Select(sc => new SlotStudent
            {
                CreatedById = accountFirebaseId,
                SlotId = addedSlot.Id,
                StudentFirebaseId = sc.StudentFirebaseId,
            });
            await _unitOfWork.SlotStudentRepository.AddRangeAsync(studentSlots);
            await _unitOfWork.SaveChangesAsync();
            return addedSlot;
        });
        await _serviceFactory.ClassService.UpdateClassStartTime(slot.ClassId);

        //Notification
        if (classDetail.IsPublic)
        {
            var notiMessage = $"Một buổi học mới đã được thêm vào lớp ${classDetail.Name} của bạn. Ngày {slot.Date}, Ca {(int)slot.Shift + 1}, Địa điểm : {room.Name}";
            var studentSlots = classDetail.StudentClasses.Select(sc => new SlotStudent
            {
                CreatedById = accountFirebaseId,
                SlotId = result.Id,
                StudentFirebaseId = sc.StudentFirebaseId,
            });
            await _serviceFactory.NotificationService.SendNotificationToManyAsync(studentSlots.Select(ss => ss.StudentFirebaseId).ToList(),
                notiMessage, "");
            if (classDetail.InstructorId != null)
            {
                await _serviceFactory.NotificationService.SendNotificationAsync(classDetail.InstructorId, notiMessage, "");
            }
        }

        return result.Adapt<SlotModel>();
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

        var notiMessage = "";
        if (updateSlotModel.RoomId != null)
        {
            var oldRoom = slot.RoomId != null ? (await  _unitOfWork.RoomRepository.GetByIdAsync(slot.RoomId.Value)) : null;
            var room = await _unitOfWork.RoomRepository.GetByIdAsync(updateSlotModel.RoomId.Value);
            if (room is null)
            {
                throw new NotFoundException("Room not found");
            }
            slot.RoomId = updateSlotModel.RoomId;
            notiMessage += $" Cập nhật phòng học từ phòng {oldRoom?.Name} sang {room.Name}";
        }

        if (updateSlotModel.Date != null || updateSlotModel.Shift != null)
        {
            notiMessage += $" Cập nhật thời gian học từ phòng ngày {slot.Date}, ca {(int)slot.Shift + 1} " +
                $"sang ngày {updateSlotModel.Date ?? slot.Date}, ca {(int)(updateSlotModel.Shift ?? slot.Shift) + 1}.";
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
            notiMessage = $"Một buổi học mới đã được cập nhật tại lớp ${classDetail.Name} của bạn." + notiMessage;
            var studentSlots = classDetail.StudentClasses.Select(sc => new SlotStudent
            {
                CreatedById = accountFirebaseId,
                SlotId = slot.Id,
                StudentFirebaseId = sc.StudentFirebaseId,
            });
            await _serviceFactory.NotificationService.SendNotificationToManyAsync(studentSlots.Select(ss => ss.StudentFirebaseId).ToList(),
                notiMessage, "");
            if (classDetail.InstructorId != null)
            {
                await _serviceFactory.NotificationService.SendNotificationAsync(classDetail.InstructorId, notiMessage, "");
            }
        }
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
            var notiMessage = $"Một buổi học đã bị xóa khỏi lớp ${classDetail.Name} của bạn. Ngày {slot.Date}, Ca {(int)slot.Shift + 1}";
            await _serviceFactory.NotificationService.SendNotificationToManyAsync(slotStudents.Select(ss => ss.StudentFirebaseId).ToList(),
                notiMessage, "");
            if (classDetail.InstructorId != null)
            {
                await _serviceFactory.NotificationService.SendNotificationAsync(classDetail.InstructorId, notiMessage, "");
            }
        }

    }
    
    public async Task<List<BlankSlotModel>> GetAllBlankSlotInWeeks(DateOnly? startDate, DateOnly? endDate)
    {
        var vietnamTime = DateTime.UtcNow.AddHours(7);
        if (startDate == null || startDate == DateOnly.FromDateTime(vietnamTime))
        {
            // Start from yesterday instead of today
            startDate = DateOnly.FromDateTime(vietnamTime.AddDays(1));
        }

        if (endDate == null)
        {
            var futureDate = vietnamTime.AddDays(7); 
            endDate = DateOnly.FromDateTime(futureDate);
        }

        // Get all active rooms
        var rooms = await _unitOfWork.RoomRepository.FindAsync(r => r.Status == RoomStatus.Opened);

        // Get all existing slots with rooms (without date filtering)
        var existingSlots = await _unitOfWork.SlotRepository.FindAsync(s => s.RoomId != null);
        
        // Filter in memory to avoid PostgreSQL date conversion issues
        var filteredSlots = existingSlots.Where(s => 
            s.Date >= startDate.Value && 
            s.Date <= endDate.Value).ToList();

        var blankSlots = new List<BlankSlotModel>();
        const int maxResults = 50;

        // Check every combination of date, shift, and room
        for (var date = startDate.Value; date <= endDate.Value; date = date.AddDays(1))
        {
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
                    bool slotExists = filteredSlots.Any(s =>
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
        }
        
        return blankSlots;
    }

    public async Task<bool> CancelSlot(CancelSlotModel model, string accountFirebaseId)
    {
        var slotInDb = await _unitOfWork.SlotRepository.FindFirstProjectedAsync<Slot>(s => s.Id == model.SlotId, hasTrackings: false);
        
        if (slotInDb == null)
        {
            throw new NotFoundException("Slot not found");
        }

        if (slotInDb.Status == SlotStatus.Cancelled)
        {
            throw new IllegalArgumentException("Slot has already been cancelled");
        }
        
        if (slotInDb.Status == SlotStatus.Finished)
        {
            throw new IllegalArgumentException("Can not cancel a finished slot");
        }
        
        if (slotInDb.Status == SlotStatus.Ongoing)
        {
            throw new IllegalArgumentException("Can not cancel an ongoing slot");
        }
        
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

        if (roomInDb.Status == RoomStatus.Closed)
        {
            throw new IllegalArgumentException("Room is closed");
        }
        
        var isConflict = await _unitOfWork.SlotRepository.AnyAsync(s => s.RoomId == model.RoomId && s.Shift == model.Shift && s.Date == model.Date);

        if (isConflict)
        {
            throw new IllegalArgumentException("Room is already open for another class");
        }
        
        // new slot in same class 
        var newSlot = new Slot
        {
            Id = Guid.NewGuid(),
            ClassId = model.ClassId,
            RoomId = model.RoomId,
            Shift = model.Shift,
            Date = model.Date,
            Status = SlotStatus.NotStarted,
        };
        
        var studentClasses = await _unitOfWork.StudentClassRepository.FindAsync(sc => sc.ClassId == model.ClassId);
        var studentIds = studentClasses.Select(x => x.StudentFirebaseId).ToList();

        var numOfStudent = studentIds.Count;
        
        var slotStudents = new List<SlotStudent>();
        
        foreach (var studentId in studentIds)
        {
            slotStudents.Add(new SlotStudent
            {
                SlotId = newSlot.Id,
                StudentFirebaseId = studentId,
                CreatedById = accountFirebaseId,
            });
        }
        
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.SlotRepository.AddAsync(newSlot);

            if (numOfStudent != slotStudents.Count)
            {
                throw new IllegalArgumentException("Number of student students are not equal, check again");
            }
            await _unitOfWork.SlotStudentRepository.AddRangeAsync(slotStudents);
        });
        
        //Notification
        await _serviceFactory.NotificationService.SendNotificationToManyAsync(studentIds, $"Một buổi học khác đã được mở tại lớp {model.ClassId} của bạn", "");
        // await InvalidateCacheForClassAsync(newSlot.ClassId, newSlot.Date);
    
        await _serviceFactory.RedisCacheService.DeleteByPatternAsync("schedule:*");
        return await GetSLotDetailById(newSlot.Id);

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
}