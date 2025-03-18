using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.Level;
using PhotonPiano.BusinessLogic.BusinessModel.Room;
using PhotonPiano.BusinessLogic.BusinessModel.Slot;
using PhotonPiano.BusinessLogic.BusinessModel.SystemConfig;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;
using PhotonPiano.Shared.Exceptions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PhotonPiano.BusinessLogic.Services;

public class ClassService : IClassService
{
    private readonly IServiceFactory _serviceFactory;
    private readonly IUnitOfWork _unitOfWork;

    public ClassService(IUnitOfWork unitOfWork, IServiceFactory serviceFactory)
    {
        _unitOfWork = unitOfWork;
        _serviceFactory = serviceFactory;
    }

    public async Task<ClassDetailModel> GetClassDetailById(Guid id)
    {
        var minStudents =
            int.Parse((await _serviceFactory.SystemConfigService.GetConfig("Sĩ số lớp tối thiểu")).ConfigValue ?? "0");
        var maxStudents =
           int.Parse((await _serviceFactory.SystemConfigService.GetConfig("Sĩ số lớp tối đa")).ConfigValue ?? "0");


        var classDetail = await _unitOfWork.ClassRepository.FindSingleProjectedAsync<ClassDetailModel>(c => c.Id == id);
        if (classDetail is null) throw new NotFoundException("Class not found");

        var level = await _unitOfWork.LevelRepository.FindSingleAsync(l => l.Id == classDetail.LevelId)
            ?? throw new NotFoundException("Level not found");

        return classDetail with { 
            MinimumStudents = minStudents,
            Capacity = maxStudents,
            InstructorName = classDetail.Instructor?.FullName,
            RequiredSlots = level.TotalSlots,
            StudentNumber = classDetail.StudentClasses.Count,     
            SlotsPerWeek = level.SlotPerWeek,
            PricePerSlots = level.PricePerSlot,
            TotalSlots = classDetail.Slots.Count,
        };
    }

    public async Task<ClassScoreboardModel> GetClassScoreboard(Guid id)
    {
        var minStudents =
            int.Parse((await _serviceFactory.SystemConfigService.GetConfig("Sĩ số lớp tối thiểu")).ConfigValue ?? "0");
        var maxStudents =
           int.Parse((await _serviceFactory.SystemConfigService.GetConfig("Sĩ số lớp tối đa")).ConfigValue ?? "0");


        var classDetail = await _unitOfWork.ClassRepository.FindSingleProjectedAsync<ClassScoreboardModel>(c => c.Id == id);
        if (classDetail is null) throw new NotFoundException("Class not found");

        var level = await _unitOfWork.LevelRepository.FindSingleAsync(l => l.Id == classDetail.LevelId)
            ?? throw new NotFoundException("Level not found");

        return classDetail with
        {
            MinimumStudents = minStudents,
            Capacity = maxStudents,
            InstructorName = classDetail.Instructor?.FullName,
            RequiredSlots = level.TotalSlots,
            StudentNumber = classDetail.StudentClasses.Count
        };
    }

    
    public async Task<List<ClassModel>> GetClassByUserFirebaseId(string userFirebaseId, Role role)
    {
        List<ClassModel> result;

        if (role == Role.Staff)
            // If the user is a Staff, return all classes
            result = await _unitOfWork.ClassRepository.FindProjectedAsync<ClassModel>();
        else
            // Otherwise, filter based on InstructorId or StudentClasses
            result = await _unitOfWork.ClassRepository.FindProjectedAsync<ClassModel>(x =>
                x.InstructorId == userFirebaseId ||
                x.StudentClasses.Any(sc => sc.StudentFirebaseId == userFirebaseId));

        if (result == null || !result.Any())
            throw new NotFoundException("Class not found");

        return result;
    }


    public async Task<PagedResult<ClassModel>> GetPagedClasses(QueryClassModel queryClass)
    {
        var (page, pageSize, sortColumn, orderByDesc,
            classStatus, level, keyword, isScorePublished, teacherId, studentId) = queryClass;

        var likeKeyword = queryClass.GetLikeKeyword();

        var query = _unitOfWork.ClassRepository.GetPaginatedWithProjectionAsQueryable<ClassModel>(
            page, pageSize, sortColumn, orderByDesc,
            expressions:
            [
                q => classStatus.Count == 0 || classStatus.Contains(q.Status),
                q => level.Count == 0 || level.Contains(q.LevelId),
                q => !isScorePublished.HasValue || q.IsScorePublished == isScorePublished,
                q => teacherId == null || q.InstructorId == teacherId,
                q => studentId == null || q.StudentClasses.Any(sc => sc.StudentFirebaseId == studentId),
                q =>
                    string.IsNullOrEmpty(keyword) ||
                    EF.Functions.ILike(EF.Functions.Unaccent(q.Name), likeKeyword)
            ]
        );
        // Fetch the class capacity
        var capacity =
            int.Parse((await _serviceFactory.SystemConfigService.GetConfig("Sĩ số lớp tối đa")).ConfigValue ?? "0");
        var minStudents =
            int.Parse((await _serviceFactory.SystemConfigService.GetConfig("Sĩ số lớp tối thiểu")).ConfigValue ?? "0");

        var levels = await _unitOfWork.LevelRepository.GetAllAsync();

        // Perform the paged query with projections
        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(q => new ClassModel
            {
                Id = q.Id,
                Name = q.Name,
                LevelId = q.LevelId,
                Status = q.Status,
                InstructorId = q.InstructorId,
                IsScorePublished = q.IsScorePublished,
                Capacity = capacity,
                CreatedById = q.CreatedById,
                IsPublic = q.IsPublic,
                Level = q.Level.Adapt<LevelModel>(),
                // Aggregate counts directly in SQL
                StudentNumber = q.StudentClasses.Count(),
                TotalSlots = q.Slots.Count(),
                Instructor = q.Instructor.Adapt<AccountSimpleModel>(),
                MinimumStudents = minStudents
            })
            .ToListAsync();

        result = result.Select(item => item with
        {
            RequiredSlots = levels.FirstOrDefault(l => l.Id == item.LevelId)?.TotalSlots ?? 0
        }).ToList();

        return new PagedResult<ClassModel>
        {
            TotalCount = await query.CountAsync(),
            Page = page,
            Limit = pageSize,
            Items = result
        };
    }

    public async Task<List<ClassModel>> AutoArrangeClasses(ArrangeClassModel arrangeClassModel, string userId)
    {
        /*===============================
         * 1. Fill students in a class
         * 2. Assign a schedule, check for available rooms (if not, pick again)
         * 3. Create slots based on the selected time and places
         * 4. Save to database
        =================================*/
        //Validation
        // Validate start week
        if (arrangeClassModel.StartWeek.DayOfWeek != DayOfWeek.Monday)
            throw new BadRequestException("Incorrect start week");

        await _serviceFactory.ProgressServiceHub.SendProgress(userId, "Đang lấy dữ liệu học viên", 0);
        //Get awaited students
        var students = await _unitOfWork.AccountRepository.FindAsync(a =>
            a.Role == Role.Student && a.StudentStatus == StudentStatus.WaitingForClass);

        var maxStudents =
            int.Parse((await _serviceFactory.SystemConfigService.GetConfig("Sĩ số lớp tối đa")).ConfigValue ?? "0");
        var minStudents =
            int.Parse((await _serviceFactory.SystemConfigService.GetConfig("Sĩ số lớp tối thiểu")).ConfigValue ?? "0");

        var classes = new List<CreateClassAutoModel>();
        //1. FILL IN STUDENTS (25%)
        //With each level, fill the students
        var levels = await _unitOfWork.LevelRepository.GetAllAsync();
        var progressLevel = 25 / (1.0 * levels.Count);
        var currentProgress = 5.0;
        await _serviceFactory.ProgressServiceHub.SendProgress(userId, "Bắt đầu xếp lớp...", currentProgress);
        foreach (var level in levels)
        {
            currentProgress += progressLevel;
            await _serviceFactory.ProgressServiceHub.SendProgress(userId, $"Bắt đầu phân bổ học viên cho LEVEL {level.Name}", currentProgress);
            await Task.Delay(100);

            var studentsOfLevel = students.Where(s => s.Level == level).ToList();
            //Find out how many classes should be created to avoid having left over students
            //Using this formular : minStudents <= Number of student need to assign / Number of classes <= maxStudents

            var numberOfStudentEachClass = 0;

            var validNumbersOfClasses = GetNumberOfClasses(minStudents, studentsOfLevel.Count, maxStudents);
            var numberOfClasses = validNumbersOfClasses.Count == 0 ? 1 : validNumbersOfClasses[0];

            if (validNumbersOfClasses.Count != 0)
            {
                numberOfStudentEachClass = (int)Math.Ceiling(studentsOfLevel.Count * 1.0 / numberOfClasses);
            }
            else
            {
                numberOfStudentEachClass = maxStudents;
                numberOfClasses = (int)Math.Floor(studentsOfLevel.Count * 1.0 / maxStudents);
            }

            
            //Great! With number of students each class and number of classes, we can easily fill in students
            for (var i = 0; i < numberOfClasses; i++)
            {
                var selectedStudents = PickRandomFromList(ref studentsOfLevel, numberOfStudentEachClass);
                classes.Add(new CreateClassAutoModel
                {
                    Id = Guid.NewGuid(),
                    LevelId = level.Id,
                    Name = "",
                    StudentIds = selectedStudents.Select(s => s.AccountFirebaseId).ToList()
                });
            }
        }
        await _serviceFactory.ProgressServiceHub.SendProgress(userId, $"Đã đến lúc phân bổ lịch học! Đang chuẩn bị cấu hình và ngày nghỉ...", currentProgress);
        await Task.Delay(500);
        //2. IT's SCHEDULE TIME!  (30%)
        Random random = new();
        //Get config values
        //Get day-offs
        var dayOffs = await _unitOfWork.DayOffRepository.FindAsync(d => d.EndTime >= DateTime.UtcNow.AddHours(7));

        //Initialize a list to store unadded slots (for conflict check)
        var unaddedSlots = new List<Slot>();

        currentProgress += 5;

        //With each class, we will pick a random schedule for it!
        var progressEachClass = 25 / (classes.Count * 1.0);
        foreach (var classDraft in classes)
        {
            await _serviceFactory.ProgressServiceHub.SendProgress(userId, $"Đang xếp lịch cho lớp {classDraft.Name}", currentProgress);
            var level = levels.FirstOrDefault(l => l.Id == classDraft.LevelId) ?? throw new NotFoundException("Level not found");
            var dayFrames = GetRandomDays(level.SlotPerWeek);
            var slot = await ScheduleAClassAutomatically(arrangeClassModel, dayOffs, dayFrames, level, unaddedSlots);
            classDraft.Slots.AddRange(slot);
            unaddedSlots.AddRange(slot.Adapt<List<Slot>>());
            currentProgress += progressEachClass;
        }

        await _serviceFactory.ProgressServiceHub.SendProgress(userId, $"Đang lưu vào cơ sở dữ liệu...", currentProgress);
        await Task.Delay(500);
        //3. Now save them to database (40%)
        var result = await SaveClasses(classes, students, userId, currentProgress, levels);


        return result;
    }

    public async Task UpdateClassStartTime(Guid classId)
    {
        var classInfo = await _unitOfWork.ClassRepository.GetByIdAsync(classId);
        var slots = await _unitOfWork.SlotRepository.FindAsync(s => s.ClassId == classId);
        if (classInfo is null)
        {
            throw new NotFoundException("Class not found");
        }
        var firstSlot = slots.OrderBy(c => c.Date).ThenBy(c => c.Shift).FirstOrDefault();
        if (firstSlot is not null)
        {
            classInfo.StartTime = firstSlot.Date;
            classInfo.UpdatedAt = DateTime.UtcNow.AddHours(7);
        }
        await _unitOfWork.ClassRepository.UpdateAsync(classInfo);
        await _unitOfWork.SaveChangesAsync();

    }
    private async Task<List<ClassModel>> SaveClasses(List<CreateClassAutoModel> classes, List<Account> students,
        string userId, double currentProgress, List<Level> levels)
    {
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            //Create classes
            await _serviceFactory.ProgressServiceHub.SendProgress(userId, $"Đang lưu các lớp...", currentProgress);
            var mappedClasses = classes.Adapt<List<Class>>();
            var monthDict = new Dictionary<int, int>();
            foreach (var classInfo in mappedClasses)
            {
                var level = levels.FirstOrDefault(l => l.Id == classInfo.LevelId) ?? throw new NotFoundException("Level not found");
                classInfo.CreatedById = userId;
                classInfo.IsPublic = false;
                classInfo.IsScorePublished = false;
                classInfo.Status = ClassStatus.NotStarted;
                classInfo.StartTime = classInfo.Slots.Count > 0 ? classInfo.Slots.First().Date : DateOnly.MinValue;

                var classesThatMonth = await _unitOfWork.ClassRepository.CountAsync(c => c.StartTime.Month == classInfo.StartTime.Month
                    && c.StartTime.Year == classInfo.StartTime.Year, false, true);

                if (monthDict.ContainsKey(classInfo.StartTime.Month))
                {
                    monthDict[classInfo.StartTime.Month] += 1;  // Increment the value
                }
                else
                {
                    monthDict[classInfo.StartTime.Month] = 1;  // Initialize if key doesn't exist
                }

                classInfo.Name = $"{level.Name.Split('(')[0]} {classesThatMonth + monthDict[classInfo.StartTime.Month] + 1} {classInfo.StartTime.Month}/{classInfo.StartTime.Year}";
            }

            await _unitOfWork.ClassRepository.AddRangeAsync(mappedClasses.Select(c =>
            {
                c.Slots = [];
                return c;
            }).ToList());
            currentProgress += 5;

            //Create StudentClasses
            await _serviceFactory.ProgressServiceHub.SendProgress(userId, $"Đang lưu danh sách học sinh các lớp...", currentProgress);
            var studentClasses = new List<StudentClass>();
            foreach (var c in classes)
                studentClasses.AddRange(c.StudentIds.Select(s => new StudentClass
                {
                    Id = Guid.NewGuid(),
                    StudentFirebaseId = s,
                    CreatedById = userId,
                    ClassId = c.Id
                }));
            await _unitOfWork.StudentClassRepository.AddRangeAsync(studentClasses);
            currentProgress += 5;

            //Create Slots
            await _serviceFactory.ProgressServiceHub.SendProgress(userId, $"Đang lưu danh sách buổi học các lớp...", currentProgress);
            var slots = new List<Slot>();
            foreach (var c in classes)
                slots.AddRange(c.Slots.Select(s => new Slot
                {
                    Id = Guid.NewGuid(),
                    ClassId = c.Id,
                    RoomId = s.RoomId,
                    Shift = s.Shift,
                    Date = s.Date,
                    Status = SlotStatus.NotStarted
                }));
            await _unitOfWork.SlotRepository.AddRangeAsync(slots);
            currentProgress += 5;

            //Create studentSlots
            await _serviceFactory.ProgressServiceHub.SendProgress(userId, $"Đang lưu các bảng điểm danh...", currentProgress);
            var studentSlots = new List<SlotStudent>();
            foreach (var studentClass in studentClasses)
            {
                var classSlots = slots.Where(s => s.ClassId == studentClass.ClassId).ToList();
                foreach (var slot in classSlots)
                    studentSlots.Add(new SlotStudent
                    {
                        CreatedById = userId,
                        SlotId = slot.Id,
                        StudentFirebaseId = studentClass.StudentFirebaseId!,
                        AttendanceStatus = AttendanceStatus.NotYet
                    });
            }
            await _unitOfWork.SlotStudentRepository.AddRangeAsync(studentSlots);
            currentProgress += 5;

            //Change student status and current class
            await _serviceFactory.ProgressServiceHub.SendProgress(userId, $"Đang cập nhật lại trạng thái các học viên...", currentProgress);
            await _unitOfWork.AccountRepository.FindAsQueryable(a =>
                    a.Role == Role.Student && a.StudentStatus == StudentStatus.WaitingForClass)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(b => b.StudentStatus, StudentStatus.InClass));
            currentProgress += 5;

            await _serviceFactory.ProgressServiceHub.SendProgress(userId, $"Đang hoàn tất...", currentProgress);
            await _unitOfWork.SaveChangesAsync();
            currentProgress += 10;

            // Fetch the class capacity
            var capacity =
                int.Parse((await _serviceFactory.SystemConfigService.GetConfig("Sĩ số lớp tối đa")).ConfigValue ?? "0");

            


            return mappedClasses.Adapt<List<ClassModel>>().Select(item => item with
            {
                Capacity = capacity,
                StudentNumber = studentClasses.Where(sc => sc.ClassId == item.Id).Count()
            }).ToList();
        });
    }


    /// <summary>
    ///     Get random days of the week ensure that each day is at least 1 day apart if possible
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private List<DayOfWeek> GetRandomDays(int n)
    {
        if (n < 1 || n > 7)
            throw new ArgumentException("n must be between 1 and 7", nameof(n));

        Random random = new();
        var allDays = Enum.GetValues<DayOfWeek>().ToList();

        // Shuffle days randomly
        allDays = allDays.OrderBy(_ => random.Next()).ToList();

        List<DayOfWeek> selectedDays = [allDays[0]]; // Start with a random first day

        while (selectedDays.Count < n)
        {
            var availableDays = allDays
                .Where(d => !selectedDays.Contains(d) && selectedDays.All(s => Math.Abs((int)s - (int)d) > 1))
                .ToList();

            // If strict spacing is no longer possible, relax the condition
            if (availableDays.Count == 0)
                availableDays = allDays.Except(selectedDays).ToList();

            if (availableDays.Count == 0)
                break; // Safety check

            selectedDays.Add(availableDays[random.Next(availableDays.Count)]);
        }

        return selectedDays;
    }

    /// <summary>
    ///     Pick randomly n elements from a list and remove it from the list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    private List<T> PickRandomFromList<T>(ref List<T> list, int n)
    {
        var rand = new Random();
        List<T> selected = [];

        if (n > list.Count)
            n = list.Count; // Ensure we don't remove more elements than available

        for (var i = 0; i < n; i++)
        {
            var index = rand.Next(list.Count); // Get random index
            selected.Add(list[index]); // Add to selected list
            list.RemoveAt(index); // Remove from original list
        }

        return selected;
    }

    private List<int> GetNumberOfClasses(double A, double B, double C)
    {
        List<int> validNs = [];

        // Solve for lower and upper bounds of N
        var minN = B / C; // Derived from B/N <= C -> N >= B/C
        var maxN = B / A; // Derived from A <= B/N -> N <= B/A

        // Convert bounds to integer range
        var start = (int)Math.Ceiling(minN); // Smallest integer >= minN
        var end = (int)Math.Floor(maxN); // Largest integer <= maxN

        // Collect valid integer values
        for (var N = start; N <= end; N++) validNs.Add(N);

        return validNs;
    }

    public async Task<ClassModel> CreateClass(CreateClassModel model, string accountFirebaseId)
    {
        var level = await _unitOfWork.LevelRepository.FindSingleAsync(level => level.Id == model.LevelId)
            ?? throw new NotFoundException("Level not found");

        var mappedClass = model.Adapt<Class>();
        mappedClass.CreatedById = accountFirebaseId;
        mappedClass.IsPublic = false;
        mappedClass.Status = ClassStatus.NotStarted;
        mappedClass.IsScorePublished = false;

        var now = DateTime.UtcNow.AddHours(7);
        var classesThisMonth = await _unitOfWork.ClassRepository.CountAsync(c => c.CreatedAt.Month == now.Month
                && c.CreatedAt.Year == now.Year && c.LevelId == model.LevelId);

        mappedClass.Name = $"{level.Name.Split('(')[0]} {classesThisMonth + 1} {now.Month}/{now.Year}";

        var addedClass = await _unitOfWork.ClassRepository.AddAsync(mappedClass);
        await _unitOfWork.SaveChangesAsync();
        return addedClass.Adapt<ClassModel>();
    }

    public async Task UpdateClass(UpdateClassModel model, string accountFirebaseId)
    {

        var classInfo = await _unitOfWork.ClassRepository.GetByIdAsync(model.Id);
        if (classInfo is null)
        {
            throw new NotFoundException("Class not found");
        }
        if (classInfo.Status != ClassStatus.NotStarted && (model.LevelId.HasValue))
        {
            throw new BadRequestException("Cannot update level of classes that are started");
        }

        var message = $"Đã có thay đổi với lớp học {classInfo.Name} của bạn.";

        if (model.Name != null && model.Name != classInfo.Name)
        {
            if (await _unitOfWork.ClassRepository.AnyAsync(c => c.Name == model.Name))
            {
                throw new ConflictException("That name is taken! Please choose an unique name!");
            }
            classInfo.Name = model.Name;
        }

        string? oldTeacherId = null;
        string oldTeacherMessage = $"Bạn đã không còn phụ trách lớp {classInfo.Name} nữa.";
        string? newTeacherMessage = null;

        if (model.LevelId.HasValue)
        {
            var level = await _unitOfWork.LevelRepository.FindSingleAsync(level => level.Id == model.LevelId)
                ?? throw new NotFoundException("Level not found");

            classInfo.LevelId = model.LevelId.Value;
        }
        if (model.InstructorId != null && model.InstructorId != classInfo.InstructorId)
        {
            var teacher = await _unitOfWork.AccountRepository.FindSingleAsync(a => a.AccountFirebaseId == model.InstructorId);
            if (teacher is null)
            {
                throw new NotFoundException("Teacher not found");
            }

            //Check instructor conflicts..
            var slotOfTeacher = await _unitOfWork.SlotRepository.Entities.Include(s => s.Class)
                .Where(s => s.Class.InstructorId == model.InstructorId 
                    && s.Class.Status != ClassStatus.Finished
                    && s.ClassId != model.Id).ToListAsync();

            var slotOfClass = await _unitOfWork.SlotRepository.FindAsync(s => s.ClassId == classInfo.Id);

            foreach (var teacherSlot in slotOfTeacher)
            {
                foreach(var classSlot in slotOfClass)
                {
                    if (teacherSlot.Shift == classSlot.Shift && teacherSlot.Date == classSlot.Date)
                    {
                        throw new ConflictException("Teacher can not be assigned to this class due to a schedule conflict");
                    }
                }
            }
            oldTeacherId = classInfo.InstructorId;
            classInfo.InstructorId = model.InstructorId;
            newTeacherMessage = $"Chúc mừng! Giờ đây bạn là giảng viên chủ nhiệm của lớp {classInfo.Name}";
        }
        classInfo.UpdateById = accountFirebaseId;
        classInfo.UpdatedAt = DateTime.UtcNow.AddHours(7);
        
        await _unitOfWork.ClassRepository.UpdateAsync(classInfo);
        await _unitOfWork.SaveChangesAsync();

        if (classInfo.IsPublic)
        {
            //send noti to teacher
            if (classInfo.InstructorId != null)
            {
                if (newTeacherMessage != null)
                {
                    await _serviceFactory.NotificationService.SendNotificationAsync(classInfo.InstructorId, newTeacherMessage, "");
                } else
                {
                    await _serviceFactory.NotificationService.SendNotificationAsync(classInfo.InstructorId, message, "");
                }
            }           
            if (oldTeacherId != null)
            {
                await _serviceFactory.NotificationService.SendNotificationAsync(oldTeacherId, oldTeacherMessage, "");
            }

            if (classInfo.InstructorId != null)
            {
                await _serviceFactory.NotificationService.SendNotificationAsync(classInfo.InstructorId, message, "");
            }
            //send noti to students
            var studentClass = await _unitOfWork.StudentClassRepository.FindAsync(sc => sc.ClassId == model.Id);
            await _serviceFactory.NotificationService.SendNotificationToManyAsync(studentClass.Select(sc => sc.StudentFirebaseId).ToList(), message, "");
        }
    }

    public async Task DeleteClass(Guid classId, string accountFirebaseId)
    {
        var classDetail = await GetClassDetailById(classId);
        if (classDetail is null)
        {
            throw new NotFoundException("Class not found");
        }

        if (classDetail.Status != ClassStatus.NotStarted)
        {
            throw new BadRequestException("Cannot delete classes that are started");
        }
        if (classDetail.IsPublic)
        {
            throw new BadRequestException("Cannot delete classes that are announced");
        }

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            //Delete slots
            var slots = classDetail.Slots.Adapt<List<Slot>>();
            foreach (var slot in slots)
            {
                slot.DeletedAt = DateTime.UtcNow.AddHours(7);
                slot.RecordStatus = RecordStatus.IsDeleted;
            }
            await _unitOfWork.SlotRepository.UpdateRangeAsync(slots);

            //Delete studentSlots
            var slotIds = slots.Select(s => s.Id).ToList();
            var studentSlots = await _unitOfWork.SlotStudentRepository.FindAsync(ss => slotIds.Contains(ss.SlotId));
            foreach (var studentSlot in studentSlots)
            {
                studentSlot.DeletedAt = DateTime.UtcNow.AddHours(7);
                studentSlot.RecordStatus = RecordStatus.IsDeleted;
            }
            await _unitOfWork.SlotStudentRepository.UpdateRangeAsync(studentSlots);

            //Delete studentClasses
            var studentClasses = classDetail.StudentClasses.Adapt<List<StudentClass>>();
            foreach (var studentClass in studentClasses)
            {
                //studentClass.Student = default;
                studentClass.DeletedAt = DateTime.UtcNow.AddHours(7);
                studentClass.RecordStatus = RecordStatus.IsDeleted;
            }
            await _unitOfWork.StudentClassRepository.UpdateRangeAsync(studentClasses);

            //Update student
            var students = classDetail.StudentClasses.Select(sc => sc.Student.Adapt<Account>()).ToList();
            foreach (var student in students)
            {
                if (student.CurrentClassId == classDetail.Id)
                {
                    student.CurrentClassId = null;
                }
                student.Level = null;//detach
                student.StudentStatus = StudentStatus.WaitingForClass;
                student.UpdatedAt = DateTime.UtcNow.AddHours(7);
            }
            await _unitOfWork.AccountRepository.UpdateRangeAsync(students);

            var classInfo = (await _unitOfWork.ClassRepository.GetByIdAsync(classId))!;
            classInfo.DeletedById = accountFirebaseId;
            classInfo.DeletedAt = DateTime.UtcNow.AddHours(7);
            classInfo.RecordStatus = RecordStatus.IsDeleted;

            await _unitOfWork.ClassRepository.UpdateAsync(classInfo);
            await _unitOfWork.SaveChangesAsync();
        });

        //if (classDetail.InstructorId != null)
        //{
        //    //send noti to teacher
        //    var message = $"Lớp học {classDetail.Name} của bạn đã bị xóa!";
        //    await _serviceFactory.NotificationService.SendNotificationAsync(classDetail.InstructorId, message, "");
        //}

    }

    public async Task PublishClass(Guid classId, string accountFirebaseId)
    {
        var classDetail = await GetClassDetailById(classId);
        var classInfo = (await _unitOfWork.ClassRepository.GetByIdAsync(classId));
        
        if (classDetail is null || classInfo is null)
        {
            throw new NotFoundException("Class not found");
        }
        
        if (classDetail.IsPublic)
        {
            throw new BadRequestException("Class is already announced!");
        }

        var level = await _unitOfWork.LevelRepository.FindSingleAsync(level => level.Id == classInfo.LevelId)
           ?? throw new NotFoundException("Level not found or removed!");

        classInfo.IsPublic = true;
        classInfo.UpdateById = accountFirebaseId;
        classInfo.UpdatedAt = DateTime.UtcNow.AddHours(7);

        //Add studentClassScore
        var studentClassScores = new List<StudentClassScore>();
        var criteria = await _unitOfWork.CriteriaRepository.FindAsync(c => c.For == CriteriaFor.Class);
        foreach (var studentClass in classDetail.StudentClasses)
        {
            foreach (var criterion in criteria)
            {
                studentClassScores.Add(new StudentClassScore
                {
                    Id = Guid.NewGuid(),
                    StudentClassId = studentClass.Id,
                    CriteriaId = criterion.Id
                });
            }
        }

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.ClassRepository.UpdateAsync(classInfo);
            await _unitOfWork.StudentClassScoreRepository.AddRangeAsync(studentClassScores);
            await _unitOfWork.SaveChangesAsync();

            // create 1st tuition for students in class
            if (classDetail.StudentClasses.Count > 0)
                await _serviceFactory.TuitionService.CreateTuitionWhenRegisterClass(classDetail);
        });

        //Notification
        if (classInfo.IsPublic)
        {
            if (classInfo.InstructorId != null)
            {
                await _serviceFactory.NotificationService.SendNotificationAsync(classInfo.InstructorId, "Thông tin lớp mới",
                $"Bạn đã được giao cho phụ trách lớp mới {classInfo.Name}, LEVEL {level.Name}. Vui lòng kiểm tra lại lịch học. Chúc bạn và lớp làm việc hiệu quả và thành công!");
            }
            await _serviceFactory.NotificationService.SendNotificationToManyAsync(classDetail.StudentClasses.Select(c => c.StudentFirebaseId).ToList(),
                $"Chúc mừng bạn đã được xếp vào lớp {classInfo.Name}, LEVEL {level.Name}. Vui lòng kiểm tra lại lịch học. Chúc bạn đạt được nhiều thành công!", "");
        }
    }

    public async Task ScheduleClass(ScheduleClassModel scheduleClassModel, string accountFirebaseId)
    {
        if (!scheduleClassModel.IsValidDayOfWeeks())
        {
            throw new BadRequestException("Days of week is not correct");
        }
        if (scheduleClassModel.StartWeek.DayOfWeek != DayOfWeek.Monday)
        {
            throw new BadRequestException("StartWeek must be monday");
        }
        if (scheduleClassModel.StartWeek <= DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7)))
        {
            throw new BadRequestException("Please choose start week in the future");
        }
        var classDetail = await GetClassDetailById(scheduleClassModel.Id);

        if (classDetail is null)
        {
            throw new NotFoundException("Class not found");
        }

        if (classDetail.Status != ClassStatus.NotStarted)
        {
            throw new BadRequestException("Class must not be started to use this feature");
        }

        if (classDetail.Slots.Count > 0)
        {
            throw new BadRequestException("Class must not have any slots to use this feature");
        }

        var level = await _unitOfWork.LevelRepository.FindSingleAsync(level => level.Id == classDetail.LevelId)
          ?? throw new NotFoundException("Level not found or removed!");

        //Construct slots and check for conflicts
        var dayOffs = await _unitOfWork.DayOffRepository.FindAsync(d => d.EndTime >= DateTime.UtcNow.AddHours(7));
        var slots = (await ScheduleAClassAutomatically(new ArrangeClassModel
        {
            AllowedShifts = [scheduleClassModel.Shift],
            StartWeek = scheduleClassModel.StartWeek,
            
        }, dayOffs, scheduleClassModel.DayOfWeeks, level));

        var mappedSlots = slots.Select(s => new Slot
        {
            Id = Guid.NewGuid(),
            Shift = s.Shift,
            Date = s.Date,
            RoomId = s.RoomId,
            ClassId = classDetail.Id
        }).ToList();
        //Add studentSlots
        var slotStudents = new List<SlotStudent>();
        foreach (var slot in mappedSlots)
        {
            foreach (var studentClass in classDetail.StudentClasses)
            {
                slotStudents.Add(new SlotStudent
                {
                    CreatedById = accountFirebaseId,
                    SlotId = slot.Id,
                    StudentFirebaseId = studentClass.StudentFirebaseId,
                    AttendanceStatus = AttendanceStatus.NotYet
                });
            }
        }
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.SlotRepository.AddRangeAsync(mappedSlots);
            await _unitOfWork.SlotStudentRepository.AddRangeAsync(slotStudents);
            await _unitOfWork.SaveChangesAsync();
        });

        //Notification
        if (classDetail.IsPublic)
        {
            var receiverIds = new List<string>();
            if (classDetail.InstructorId != null)
            {
                receiverIds.Add(classDetail.InstructorId);
            }
            receiverIds.AddRange(classDetail.StudentClasses.Select(s => s.StudentFirebaseId).ToList());

            await _serviceFactory.NotificationService.SendNotificationToManyAsync(receiverIds,
                $"Lớp {classDetail.Name} đã cập nhật lịch học mới. Vui lòng kiểm tra lại. Chúc bạn đạt được nhiều thành công!", "");
        }
        
    }

    private async Task<List<CreateSlotThroughArrangementModel>> ScheduleAClassAutomatically(
        ArrangeClassModel arrangeClassModel, 
        List<DayOff> dayOffs, List<DayOfWeek> dayFrames, 
        Level level,
        List<Slot>? otherSlots = null)
    {
        otherSlots ??= [];
        var r = new Random();
        //Pick a random shift in the passed shift list
        var pickedShift = arrangeClassModel.AllowedShifts[r.Next(arrangeClassModel.AllowedShifts.Count - 1)];

        int maxAttempt = 100, attempt = 1; //Avoid infinite loop
        var availableRooms = new List<RoomModel>();

        var dates = new HashSet<DateOnly>();
        var currentDate = arrangeClassModel.StartWeek;
        int slotCount = 0;
        while (slotCount < level.TotalSlots)
        {
            foreach (var frame in dayFrames)
            {
                var date = currentDate.AddDays((int)frame);

                if (!dayOffs.Any(dayOff =>
                        date.ToDateTime(TimeOnly.MinValue) >= dayOff.StartTime &&
                        date.ToDateTime(TimeOnly.MaxValue) <= dayOff.EndTime))
                {
                    dates.Add(date);
                    slotCount++;

                    if (slotCount >= level.TotalSlots)
                        break;
                }
            }
            currentDate = currentDate.AddDays(7); // Move to the next week
        }

        //Check available rooms -- If not, iterate
        while (availableRooms.Count == 0 && attempt < maxAttempt)
        {
            availableRooms = await _serviceFactory.RoomService.GetAvailableRooms(pickedShift, dates, otherSlots);
            attempt++;
            if (attempt >= maxAttempt)
                throw new ConflictException(
                    "Unable to complete arranging classes! No available rooms found! Consider different start week or change the shift range");
        }
        
        

        //Pick a room
        var room = availableRooms[r.Next(availableRooms.Count - 1)];
        var result = dates.Select(d => new CreateSlotThroughArrangementModel
        {
            Date = d,
            Shift = pickedShift,
            RoomId = room.Id
        }).OrderBy(s => s.Date).ThenBy(s => s.Shift).ToList();

        return result;
    }
}