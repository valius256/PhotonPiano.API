using Mapster;
using Microsoft.EntityFrameworkCore;
using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.Level;
using PhotonPiano.BusinessLogic.BusinessModel.Room;
using PhotonPiano.BusinessLogic.BusinessModel.Slot;
using PhotonPiano.BusinessLogic.BusinessModel.Utils;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;
using PhotonPiano.Shared.Enums;
using PhotonPiano.Shared.Exceptions;
using PhotonPiano.Shared.Utils;

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
            int.Parse((await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.MinimumStudents)).ConfigValue ??
                      "0");
        var maxStudents =
            int.Parse((await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.MaximumStudents)).ConfigValue ??
                      "0");


        var classDetail = await _unitOfWork.ClassRepository.FindSingleProjectedAsync<ClassDetailModel>(c => c.Id == id);
        if (classDetail is null) throw new NotFoundException("Class not found");

        var level = await _unitOfWork.LevelRepository.FindSingleAsync(l => l.Id == classDetail.LevelId)
                    ?? throw new NotFoundException("Level not found");

        return classDetail with
        {
            MinimumStudents = minStudents,
            Capacity = maxStudents,
            InstructorName = classDetail.Instructor?.FullName,
            RequiredSlots = level.TotalSlots,
            StudentNumber = classDetail.StudentClasses.Count,
            SlotsPerWeek = level.SlotPerWeek,
            PricePerSlots = level.PricePerSlot,
            TotalSlots = classDetail.Slots.Count
        };
    }

    public async Task<ClassScoreboardModel> GetClassScoreboard(Guid id)
    {
        var minStudents =
            int.Parse((await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.MinimumStudents)).ConfigValue ??
                      "0");
        var maxStudents =
            int.Parse((await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.MaximumStudents)).ConfigValue ??
                      "0");


        var classDetail =
            await _unitOfWork.ClassRepository.FindSingleProjectedAsync<ClassScoreboardModel>(c => c.Id == id);
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
            classStatus, level, keyword, isScorePublished, teacherId, studentId, isPublic) = queryClass;

        var likeKeyword = queryClass.GetLikeKeyword();

        var query = _unitOfWork.ClassRepository.GetPaginatedWithProjectionAsQueryable<ClassWithSlotsModel>(
            page, pageSize, sortColumn, orderByDesc,
            expressions:
            [
                q => classStatus.Count == 0 || classStatus.Contains(q.Status),
                q => level.Count == 0 || level.Contains(q.LevelId),
                q => !isScorePublished.HasValue || q.IsScorePublished == isScorePublished,
                q => teacherId == null || q.InstructorId == teacherId,
                q => studentId == null || (q.StudentClasses.Any(sc => sc.StudentFirebaseId == studentId) && q.IsPublic),
                q => isPublic == null || q.IsPublic == isPublic,
                q =>
                    string.IsNullOrEmpty(keyword) ||
                    EF.Functions.ILike(EF.Functions.Unaccent(q.Name), likeKeyword) ||
                    (q.ScheduleDescription != null && EF.Functions.ILike(EF.Functions.Unaccent(q.ScheduleDescription), likeKeyword))
            ]
        );
        // Fetch the class capacity
        var capacity =
            int.Parse((await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.MaximumStudents)).ConfigValue ??
                      "0");
        var minStudents =
            int.Parse((await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.MinimumStudents)).ConfigValue ??
                      "0");

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
                ScheduleDescription = q.ScheduleDescription,
                StartTime = q.Slots.Min(s => (DateOnly?)s.Date),
                EndTime = q.Slots.Max(s => (DateOnly?)s.Date),
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

        await _serviceFactory.ProgressServiceHub.SendProgress(userId, "Gathering learner data", 0);
        //Get awaited students
        var students = await _unitOfWork.AccountRepository.Entities
            .Include(a => a.FreeSlots)
            .Where(a => a.Role == Role.Student && a.StudentStatus == StudentStatus.WaitingForClass)
            .ToListAsync();

        var maxStudents =
            int.Parse((await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.MaximumStudents)).ConfigValue ??
                      "0");
        var minStudents =
            int.Parse((await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.MinimumStudents)).ConfigValue ??
                      "0");

        var classes = new List<CreateClassAutoModel>();
        //1. FILL IN STUDENTS (25%)
        //With each level, fill the students
        var levels = await _unitOfWork.LevelRepository.GetAllAsync();
        var progressLevel = 25 / (1.0 * levels.Count);
        var currentProgress = 5.0;
        await _serviceFactory.ProgressServiceHub.SendProgress(userId, "Starting...", currentProgress);
        foreach (var level in levels)
        {
            currentProgress += progressLevel;
            await _serviceFactory.ProgressServiceHub.SendProgress(userId, $"Assigning learners of LEVEL {level.Name}",
                currentProgress);
            await Task.Delay(100);

            var studentsOfLevel = students.Where(s => s.Level == level).ToList();

            // Build Graph based on shared FreeSlots
            var studentGraph = new FreeSlotGraph();

            for (var i = 0; i < studentsOfLevel.Count; i++)
            for (var j = i + 1; j < studentsOfLevel.Count; j++)
            {
                var commonSlots = studentsOfLevel[i].FreeSlots
                    .Select(f => (f.DayOfWeek, f.Shift))
                    .Intersect(studentsOfLevel[j].FreeSlots.Select(f => (f.DayOfWeek, f.Shift)))
                    .ToList();

                if (commonSlots.Count >= 2)
                    studentGraph.AddEdge(studentsOfLevel[i].AccountFirebaseId, studentsOfLevel[j].AccountFirebaseId);
            }

            // Find valid groups (Cliques)
            var studentGroups = studentGraph.FindCliques(minStudents, maxStudents);
            var unassignedStudents = new HashSet<string>(studentsOfLevel.Select(s => s.AccountFirebaseId));

            // Assign students to classes
            foreach (var group in studentGroups)
            foreach (var student in group)
                unassignedStudents.Remove(student);
            // 🔹 Handle Unassigned Students
            var unassignedList = unassignedStudents.ToList();
            while (unassignedList.Count != 0)
            {
                var student = unassignedList.First();
                unassignedList.RemoveAt(0);

                // Try adding to an existing group
                var bestGroup = studentGroups
                    .Where(g => g.Count < maxStudents)
                    .OrderByDescending(g => g.Count) // Prefer nearly full groups
                    .FirstOrDefault();

                if (bestGroup != null)
                    bestGroup.Add(student);
                else
                    // Create a new group if necessary
                    studentGroups.Add([student]);
            }

            // 🔹 Remove any groups that don't meet minStudents
            studentGroups.RemoveAll(g => g.Count < minStudents);

            foreach (var group in studentGroups)
                classes.Add(new CreateClassAutoModel
                {
                    Id = Guid.NewGuid(),
                    LevelId = level.Id,
                    Name = "",
                    StudentIds = group
                });
        }

        await _serviceFactory.ProgressServiceHub.SendProgress(userId,
            "Time to schedule! Preparing configuration and holidays...", currentProgress);
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
        for (var i = 0; i < classes.Count; i++)
        {
            var classDraft = classes[i];

            await _serviceFactory.ProgressServiceHub.SendProgress(userId, $"Scheduling {classDraft.Name}",
                currentProgress);
            var level = levels.FirstOrDefault(l => l.Id == classDraft.LevelId) ??
                        throw new NotFoundException("Level not found");
            var slotsPerWeek = level.SlotPerWeek;

            // 🔹 Get all students in the class
            var studentsInClass = students
                .Where(s => classDraft.StudentIds.Contains(s.AccountFirebaseId))
                .ToList();

            // 🔹 Count occurrences of each (DayOfWeek, Shift) pair
            var slotCounts = new Dictionary<(DayOfWeek, Shift), int>();

            foreach (var student in studentsInClass)
            foreach (var freeSlot in student.FreeSlots)
            {
                var key = (freeSlot.DayOfWeek, freeSlot.Shift);
                if (!slotCounts.ContainsKey(key))
                    slotCounts[key] = 0;
                slotCounts[key]++;
            }

            // 🔹 Sort slots by highest student availability
            var bestSlots = slotCounts
                .OrderByDescending(kvp => kvp.Value) // Sort by most students available
                .Take(slotsPerWeek) // Pick the top `slotsPerWeek`
                .Select(kvp => kvp.Key)
                .ToList();

            var slots = await ScheduleAClassAutomatically(arrangeClassModel.StartWeek, bestSlots, dayOffs, level,
                unaddedSlots, false);

            //var (slot, shift) = 
            classDraft.Slots.AddRange(slots.Slots.Adapt<List<CreateSlotThroughArrangementModel>>());
            unaddedSlots.AddRange(slots.Slots.Adapt<List<Slot>>());

            var scheduleDescription = "";
            if (slots.Slots.Count == 0)
                scheduleDescription = "Chưa xác định";
            else
                foreach (var timeSlot in slots.FinalFrame)
                    scheduleDescription +=
                        $"{Constants.VietnameseDaysOfTheWeek[(int)timeSlot.Item1]} Shift {(int)timeSlot.Item2 + 1} ({Constants.Shifts[(int)timeSlot.Item2]}); ";

            // Assign the modified instance back to the list
            classes[i] = classDraft with
            {
                ScheduleDescription = scheduleDescription
            };

            currentProgress += progressEachClass;
        }

        await _serviceFactory.ProgressServiceHub.SendProgress(userId, "Saving to database...", currentProgress);
        await Task.Delay(500);
        //3. Now save them to database (40%)
        var result = await SaveClasses(classes, userId, currentProgress, levels);


        return result;
    }

    public async Task UpdateClassStartTime(Guid classId)
    {
        var classInfo = await _unitOfWork.ClassRepository.GetByIdAsync(classId);
        var slots = await _unitOfWork.SlotRepository.FindAsync(s => s.ClassId == classId);
        if (classInfo is null) throw new NotFoundException("Class not found");
        var firstSlot = slots.OrderBy(c => c.Date).ThenBy(c => c.Shift).FirstOrDefault();
        if (firstSlot is not null)
        {
            classInfo.StartTime = firstSlot.Date;
            classInfo.UpdatedAt = DateTime.UtcNow.AddHours(7);
        }

        await _unitOfWork.ClassRepository.UpdateAsync(classInfo);
        await _unitOfWork.SaveChangesAsync();
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
            && c.CreatedAt.Year == now.Year && c.LevelId == model.LevelId, false, true);

        mappedClass.Name = $"{level.Name.Split('(')[0]}_{classesThisMonth + 1}_{now.Month}/{now.Year}";

        var addedClass = await _unitOfWork.ClassRepository.AddAsync(mappedClass);
        await _unitOfWork.SaveChangesAsync();
        return addedClass.Adapt<ClassModel>();
    }

    public async Task UpdateClass(UpdateClassModel model, string accountFirebaseId)
    {
        var classInfo = await _unitOfWork.ClassRepository.GetByIdAsync(model.Id);
        if (classInfo is null) throw new NotFoundException("Class not found");
        if (classInfo.Status != ClassStatus.NotStarted && model.LevelId.HasValue && model.LevelId != classInfo.LevelId)
            throw new BadRequestException("Cannot update level of classes that are started");

        var message = $"There is a change to your class {classInfo.Name}.";

        if (model.Name != null && model.Name != classInfo.Name)
        {
            if (await _unitOfWork.ClassRepository.AnyAsync(c => c.Name == model.Name))
                throw new ConflictException("That name is taken! Please choose an unique name!");
            classInfo.Name = model.Name;
        }

        if (model.ScheduleDescription != null) classInfo.ScheduleDescription = model.ScheduleDescription;

        string? oldTeacherId = null;
        var oldTeacherMessage = $"You are no longer teaching {classInfo.Name}.";
        string? newTeacherMessage = null;

        if (model.LevelId.HasValue && model.LevelId.Value != classInfo.LevelId)
        {
            if (await _unitOfWork.StudentClassRepository.AnyAsync(sc => sc.ClassId == classInfo.Id))
                throw new BadRequestException("Can't update level of this class because there are students in it!");
            var level = await _unitOfWork.LevelRepository.FindSingleAsync(level => level.Id == model.LevelId)
                        ?? throw new NotFoundException("Level not found");

            classInfo.LevelId = model.LevelId.Value;
        }

        var slotOfTeacher = new List<Slot>();
        var slotOfClass = new List<Slot>();
        if (model.InstructorId != null && model.InstructorId != classInfo.InstructorId)
        {
            var teacher =
                await _unitOfWork.AccountRepository.FindSingleAsync(a => a.AccountFirebaseId == model.InstructorId);
            if (teacher is null) throw new NotFoundException("Teacher not found");

            //Check instructor conflicts..
            slotOfTeacher = await _unitOfWork.SlotRepository.Entities.Include(s => s.Class)
                .Where(s => s.TeacherId == teacher.AccountFirebaseId
                            && s.Status != SlotStatus.Finished)
                .AsNoTracking()
                .ToListAsync();

            slotOfClass = await _unitOfWork.SlotRepository.FindAsync(s => s.ClassId == classInfo.Id, false);


            foreach (var classSlot in slotOfClass)
            {
                foreach (var teacherSlot in slotOfTeacher)
                    if (teacherSlot.Shift == classSlot.Shift && teacherSlot.Date == classSlot.Date &&
                        classSlot.TeacherId != teacher.AccountFirebaseId)
                        throw new ConflictException(
                            "Teacher can not be assigned to this class due to a schedule conflict");

                classSlot.TeacherId = teacher.AccountFirebaseId;
            }

            //Update slot

            oldTeacherId = classInfo.InstructorId;
            classInfo.InstructorId = model.InstructorId;
            newTeacherMessage = $"Congratulations! You are now the homeroom teacher of the class {classInfo.Name}";
        }

        classInfo.UpdateById = accountFirebaseId;
        classInfo.UpdatedAt = DateTime.UtcNow.AddHours(7);

        //Update
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.ClassRepository.UpdateAsync(classInfo);
            await _unitOfWork.SlotRepository.UpdateRangeAsync(slotOfClass);
            await _unitOfWork.SaveChangesAsync();
        });


        if (classInfo.IsPublic)
        {
            //send noti to teacher
            if (classInfo.InstructorId != null)
            {
                if (newTeacherMessage != null)
                    await _serviceFactory.NotificationService.SendNotificationAsync(classInfo.InstructorId,
                        newTeacherMessage, "");
                else
                    await _serviceFactory.NotificationService.SendNotificationAsync(classInfo.InstructorId, message,
                        "");
            }

            if (oldTeacherId != null)
                await _serviceFactory.NotificationService.SendNotificationAsync(oldTeacherId, oldTeacherMessage, "");

            if (classInfo.InstructorId != null)
                await _serviceFactory.NotificationService.SendNotificationAsync(classInfo.InstructorId, message, "");
            //send noti to students
            var studentClass = await _unitOfWork.StudentClassRepository.FindAsync(sc => sc.ClassId == model.Id);
            await _serviceFactory.NotificationService.SendNotificationToManyAsync(
                studentClass.Select(sc => sc.StudentFirebaseId).ToList(), message, "");
        }
    }

    public async Task DeleteClass(Guid classId, string accountFirebaseId)
    {
        var classDetail = await GetClassDetailById(classId);
        if (classDetail is null) throw new NotFoundException("Class not found");

        if (classDetail.Status != ClassStatus.NotStarted)
            throw new BadRequestException("Cannot delete classes that are started");
        if (classDetail.IsPublic) throw new BadRequestException("Cannot delete classes that are announced");

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
                if (student.CurrentClassId == classDetail.Id) student.CurrentClassId = null;
                student.Level = null; //detach
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
        var classInfo = await _unitOfWork.ClassRepository.GetByIdAsync(classId);

        if (classDetail is null || classInfo is null) throw new NotFoundException("Class not found");

        if (classDetail.IsPublic) throw new BadRequestException("Class is already announced!");

        var level = await _unitOfWork.LevelRepository.FindSingleAsync(level => level.Id == classInfo.LevelId)
                    ?? throw new NotFoundException("Level not found or removed!");

        //var minSizeConfig = await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.MinimumStudents);
        //if (minSizeConfig is not null)
        //    if (classDetail.StudentClasses.Count < int.Parse(minSizeConfig.ConfigValue ?? "0"))
        //        throw new BadRequestException(
        //            "Can't publish the class because the minimum class size is not statisfied");

        if (classDetail.Slots.Count < level.TotalSlots)
            throw new BadRequestException(
                "Can't publish the class because the total amount of slots is not statisfied");

        classInfo.IsPublic = true;
        classInfo.UpdateById = accountFirebaseId;
        classInfo.UpdatedAt = DateTime.UtcNow.AddHours(7);

        //Add studentClassScore
        var studentClassScores = new List<StudentClassScore>();
        var criteria = await _unitOfWork.CriteriaRepository.FindAsync(c => c.For == CriteriaFor.Class);
        foreach (var studentClass in classDetail.StudentClasses)
        foreach (var criterion in criteria)
            studentClassScores.Add(new StudentClassScore
            {
                Id = Guid.NewGuid(),
                StudentClassId = studentClass.Id,
                CriteriaId = criterion.Id
            });

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.ClassRepository.UpdateAsync(classInfo);
            await _unitOfWork.StudentClassScoreRepository.AddRangeAsync(studentClassScores);
            await _unitOfWork.SaveChangesAsync();
        });

        // create tuition when class is published
        await _serviceFactory.TuitionService.CreateTuitionWhenRegisterClass(classDetail);

        //Notification
        if (classInfo.IsPublic)
        {
            if (classInfo.InstructorId != null)
                await _serviceFactory.NotificationService.SendNotificationAsync(classInfo.InstructorId,
                    "New Class Information",
                    $"You're assigned to class {classInfo.Name}, LEVEL {level.Name}. Please check your schedule! Wish you and your class a success work");
            await _serviceFactory.NotificationService.SendNotificationToManyAsync(
                classDetail.StudentClasses.Select(c => c.StudentFirebaseId).ToList(),
                $"Congratulations! You have been assigned to class {classInfo.Name}, LEVEL {level.Name}. Please check your schedule! Best Wishes!",
                "");
        }
    }

    public async Task ScheduleClass(ScheduleClassModel scheduleClassModel, string accountFirebaseId)
    {
        if (!scheduleClassModel.IsValidDayOfWeeks()) throw new BadRequestException("Days of week is not correct");
        if (scheduleClassModel.StartWeek.DayOfWeek != DayOfWeek.Monday)
            throw new BadRequestException("StartWeek must be monday");
        if (scheduleClassModel.StartWeek <= DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7)))
            throw new BadRequestException("Please choose start week in the future");
        var classDetail = await GetClassDetailById(scheduleClassModel.Id);

        if (classDetail is null) throw new NotFoundException("Class not found");

        if (classDetail.Status != ClassStatus.NotStarted)
            throw new BadRequestException("Class must not be started to use this feature");

        if (classDetail.Slots.Count > 0)
            throw new BadRequestException("Class must not have any slots to use this feature");

        var level = await _unitOfWork.LevelRepository.FindSingleAsync(level => level.Id == classDetail.LevelId)
                    ?? throw new NotFoundException("Level not found or removed!");

        if (level.SlotPerWeek != scheduleClassModel.DayOfWeeks.Count)
            throw new BadRequestException("Number of slots per week doesn't statisfy the config requirement of " +
                                          level.SlotPerWeek);
        //Construct slots and check for conflicts
        var dayOffs = await _unitOfWork.DayOffRepository.FindAsync(d => d.EndTime >= DateTime.UtcNow.AddHours(7));
        var requestTimeSlots = scheduleClassModel.DayOfWeeks.Select(dow => (dow, scheduleClassModel.Shift)).ToList();
        var (slots, finalFrames) =
            await ScheduleAClassAutomatically(scheduleClassModel.StartWeek, requestTimeSlots, dayOffs, level, []);

        var mappedSlots = slots.Select(s => new Slot
        {
            Id = s.Id,
            Shift = s.Shift,
            Date = s.Date,
            RoomId = s.RoomId,
            ClassId = classDetail.Id
        }).ToList();
        //Add studentSlots
        var slotStudents = new List<SlotStudent>();
        foreach (var slot in mappedSlots)
        foreach (var studentClass in classDetail.StudentClasses)
            slotStudents.Add(new SlotStudent
            {
                CreatedById = accountFirebaseId,
                SlotId = slot.Id,
                StudentFirebaseId = studentClass.StudentFirebaseId,
                AttendanceStatus = AttendanceStatus.NotYet
            });

        //Update schedule description
        var scheduleDescription = "";
        foreach (var timeSlot in finalFrames)
            scheduleDescription +=
                $"{Constants.VietnameseDaysOfTheWeek[(int)timeSlot.Item1]} Shift {(int)timeSlot.Item2 + 1} ({Constants.Shifts[(int)timeSlot.Item2]}); ";
        var classStartDate = classDetail.Slots.Count > 0 ? classDetail.Slots.First().Date : DateOnly.MaxValue;
        //Save change
        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.SlotRepository.AddRangeAsync(mappedSlots);
            await _unitOfWork.SlotStudentRepository.AddRangeAsync(slotStudents);
            await _unitOfWork.ClassRepository.ExecuteUpdateAsync(
                c => c.Id == classDetail.Id,
                set => set
                    .SetProperty(c => c.ScheduleDescription, scheduleDescription)
                    .SetProperty(c => c.StartTime, classStartDate)
            );
            await _unitOfWork.SaveChangesAsync();
        });

        //Notification
        if (classDetail.IsPublic)
        {
            var receiverIds = new List<string>();
            if (classDetail.InstructorId != null) receiverIds.Add(classDetail.InstructorId);
            receiverIds.AddRange(classDetail.StudentClasses.Select(s => s.StudentFirebaseId).ToList());

            await _serviceFactory.NotificationService.SendNotificationToManyAsync(receiverIds,
                $"Class {classDetail.Name} has updated its schedule. Please check again. Regards!", "");
        }
    }

    public async Task ClearClassSchedule(Guid classId, string accountFirebaseId)
    {
        var classDetail = await _unitOfWork.ClassRepository.Entities
            .Include(c => c.Slots)
            .ThenInclude(s => s.SlotStudents)
            .SingleOrDefaultAsync(c => c.Id == classId);

        if (classDetail is null) throw new NotFoundException("Class not found");
        if (classDetail.Status != ClassStatus.NotStarted)
            throw new BadRequestException("Cannot clear schedule of classes that are started");
        if (classDetail.IsPublic) throw new BadRequestException("Cannot clear schedule of classes that are announced");
        var deleteSlots = new List<Slot>();
        var deleteStudentSlots = new List<SlotStudent>();
        foreach (var slot in classDetail.Slots)
            if (slot.Status == SlotStatus.NotStarted)
            {
                deleteSlots.Add(slot);
                deleteStudentSlots.AddRange(slot.SlotStudents);
            }

        var classInfo = await _unitOfWork.ClassRepository.GetByIdAsync(classId) ??
                        throw new NotFoundException("Class not found");
        classInfo.UpdateById = accountFirebaseId;
        classInfo.UpdatedAt = DateTime.UtcNow.AddHours(7);

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.SlotStudentRepository.DeleteRangeAsync(deleteStudentSlots);
            await _unitOfWork.SlotRepository.DeleteRangeAsync(deleteSlots);
            await _unitOfWork.ClassRepository.UpdateAsync(classInfo);
        });
    }

    public async Task<PagedResult<AccountSimpleModel>> GetAvailableTeacher(GetAvailableTeacherForClassModel model)
    {
        var classEntity = await _unitOfWork.ClassRepository
            .Entities.Include(c => c.Slots).Include(c => c.Instructor).FirstOrDefaultAsync(x => x.Id == model.ClassId);

        if (classEntity is null) throw new NotFoundException("Class not found");

        //if (!classEntity.Slots.Any()) throw new NotFoundException("Class with no slots can't check conflict");

        var classSlots = classEntity.Slots;
        //var startDate = classSlots.Min(x => x.Date);
        //var endDate = classSlots.Max(x => x.Date);

        var (page, pageSize, sortColumn, orderByDesc, classId, keyword) = model;

        if (string.IsNullOrEmpty(sortColumn) || sortColumn == "Id") sortColumn = "FullName";

        //// Get all teachers with pagination
        //var teacherQuery = _unitOfWork.AccountRepository.GetPaginatedWithProjectionAsQueryable<AccountSimpleModel>(
        //    page, pageSize, sortColumn, orderByDesc,
        //    expressions: [q => q.Role == Role.Instructor]
        //);

        //var totalCount = await teacherQuery.CountAsync();
        //var teachers = await teacherQuery.ToListAsync();

        //// Get all slots in the same time period that have teacher assignments
        //var occupiedSlots = await _unitOfWork.SlotRepository.FindAsync(x =>
        //    x.Date >= startDate && x.Date <= endDate && x.TeacherId != null);

        //// Group slots by teacher ID
        //var slotsByTeacher = occupiedSlots.GroupBy(s => s.TeacherId)
        //    .ToDictionary(g => g.Key!, g => g.ToList());

        //// Find teachers without conflicts
        //var result = new TeacherWithSlotModel { TeacherWithSlots = new List<TeacherFitWithSlotModel>() };

        //foreach (var teacher in teachers)
        //{
        //    var hasConflict = false;

        //    // Check if teacher has any conflicts with class slots
        //    if (slotsByTeacher.TryGetValue(teacher.AccountFirebaseId, out var teacherSlots))
        //        foreach (var classSlot in classSlots)
        //            if (teacherSlots.Any(ts => ts.Date == classSlot.Date && ts.Shift == classSlot.Shift))
        //            {
        //                hasConflict = true;
        //                break;
        //            }

        //    // Only add teachers with no conflicts
        //    if (!hasConflict)
        //        result.TeacherWithSlots.Add(new TeacherFitWithSlotModel
        //        {
        //            TeacherId = teacher.AccountFirebaseId,
        //            TeacherName = teacher.FullName ?? teacher.UserName,
        //            Slots = classSlots.Adapt<List<SlotWithInforModel>>(),
        //            TotalSlots = classSlots.Count
        //        });
        //}
        var likeKeyword = model.GetLikeKeyword();

        // Flatten class slot dates and shifts into primitive comparison sets
        var slotDates = classSlots.Select(s => s.Date).ToList();
        var slotShifts = classSlots.Select(s => s.Shift).ToList();

        var result = await _unitOfWork.AccountRepository.GetPaginatedAsync(
            page, pageSize, sortColumn, orderByDesc,
            expressions: [q =>
                q.Role == Role.Instructor 
                && !q.Teacherslots.Any(s => slotDates.Contains(s.Date) && slotShifts.Contains(s.Shift) && s.ClassId != classId)
                && (keyword == null || EF.Functions.ILike(EF.Functions.Unaccent(q.FullName ?? q.UserName ?? string.Empty),likeKeyword))]
        );
        return result.Adapt<PagedResult<AccountSimpleModel>>();
        //return new PagedResult<TeacherWithSlotModel>
        //{
        //    Items = [result],
        //    Page = model.Page,
        //    Limit = model.PageSize,
        //    TotalCount = totalCount
        //};
    }

    private async Task<List<ClassModel>> SaveClasses(List<CreateClassAutoModel> classes,
        string userId, double currentProgress, List<Level> levels)
    {
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            //Create classes
            await _serviceFactory.ProgressServiceHub.SendProgress(userId, "Saving Classes...", currentProgress);
            var mappedClasses = classes.Adapt<List<Class>>();
            var monthLevelDict = new Dictionary<(int Month, int Year, Guid LevelId), int>();
            foreach (var classInfo in mappedClasses)
            {
                var level = levels.FirstOrDefault(l => l.Id == classInfo.LevelId) ??
                            throw new NotFoundException("Level not found");
                classInfo.CreatedById = userId;
                classInfo.IsPublic = false;
                classInfo.IsScorePublished = false;
                classInfo.Status = ClassStatus.NotStarted;
                classInfo.StartTime = classInfo.Slots.Count > 0 ? classInfo.Slots.First().Date : DateOnly.MaxValue;

                var classesThatMonth = await _unitOfWork.ClassRepository.CountAsync(c =>
                    c.StartTime.Month == classInfo.StartTime.Month
                    && c.StartTime.Year == classInfo.StartTime.Year && c.LevelId == level.Id, false, true);

                var key = (classInfo.StartTime.Month, classInfo.StartTime.Year, level.Id);

                if (monthLevelDict.ContainsKey(key))
                    monthLevelDict[key] += 1;
                else
                    monthLevelDict[key] = 1;

                classInfo.Name =
                    $"{level.Name.Split('(')[0]}_{classesThatMonth + monthLevelDict[key]}_{classInfo.StartTime.Month}/{classInfo.StartTime.Year}";
            }

            await _unitOfWork.ClassRepository.AddRangeAsync(mappedClasses.Select(c =>
            {
                c.Slots = [];
                return c;
            }).ToList());
            currentProgress += 5;

            //Create StudentClasses
            //await _serviceFactory.ProgressServiceHub.SendProgress(userId, "Saving learners of classes...",
            //    currentProgress);
            //var studentClasses = new List<StudentClass>();
            //foreach (var c in classes)
            //    studentClasses.AddRange(c.StudentIds.Select(s => new StudentClass
            //    {
            //        Id = Guid.NewGuid(),
            //        StudentFirebaseId = s,
            //        CreatedById = userId,
            //        ClassId = c.Id
            //    }));
            //await _unitOfWork.StudentClassRepository.AddRangeAsync(studentClasses);
            currentProgress += 5;

            //Create Slots
            await _serviceFactory.ProgressServiceHub.SendProgress(userId, "Saving slots...", currentProgress);
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
            //await _serviceFactory.ProgressServiceHub.SendProgress(userId, "Saving slot data...", currentProgress);
            //var studentSlots = new List<SlotStudent>();
            //foreach (var studentClass in studentClasses)
            //{
            //    var classSlots = slots.Where(s => s.ClassId == studentClass.ClassId).ToList();
            //    foreach (var slot in classSlots)
            //        studentSlots.Add(new SlotStudent
            //        {
            //            CreatedById = userId,
            //            SlotId = slot.Id,
            //            StudentFirebaseId = studentClass.StudentFirebaseId!,
            //            AttendanceStatus = AttendanceStatus.NotYet
            //        });
            //}

            //await _unitOfWork.SlotStudentRepository.AddRangeAsync(studentSlots);
            currentProgress += 5;

            //Change student status and current class
            //await _serviceFactory.ProgressServiceHub.SendProgress(userId, "Updating status of learners...",
            //    currentProgress);
            //var studentToUpdate = await _unitOfWork.AccountRepository.FindAsQueryable(a =>
            //    a.Role == Role.Student && a.StudentStatus == StudentStatus.WaitingForClass).ToListAsync();

            //foreach (var student in studentToUpdate)
            //{
            //    var studentClass =
            //        studentClasses.FirstOrDefault(sc => sc.StudentFirebaseId == student.AccountFirebaseId);
            //    if (studentClass != null)
            //    {
            //        student.StudentStatus = StudentStatus.InClass;
            //        student.CurrentClassId = studentClass.ClassId;
            //    }
            //}

            //    .ExecuteUpdateAsync(setters => setters
            //        .SetProperty(b => b.StudentStatus, StudentStatus.InClass)
            //        .SetProperty(b => b.CurrentClassId, cla);
            //await _unitOfWork.AccountRepository.UpdateRangeAsync(studentToUpdate);
            //currentProgress += 5;

            await _serviceFactory.ProgressServiceHub.SendProgress(userId, "Completing...", currentProgress);
            await _unitOfWork.SaveChangesAsync();
            currentProgress += 10;

            // Fetch the class capacity
            var capacity =
                int.Parse(
                    (await _serviceFactory.SystemConfigService.GetConfig(ConfigNames.MaximumStudents)).ConfigValue ??
                    "0");


            return mappedClasses.Adapt<List<ClassModel>>().Select(item => item with
            {
                Capacity = capacity,
                //StudentNumber = studentClasses.Where(sc => sc.ClassId == item.Id).Count()
            }).ToList();
        });
    }

    private async Task<(List<SlotModel> Slots, List<(DayOfWeek, Shift)> FinalFrame)> ScheduleAClassAutomatically(
        DateOnly startWeek, List<(DayOfWeek, Shift)> frames,
        List<DayOff> dayOffs,
        Level level,
        List<Slot>? otherSlots = null, bool breakWhenFail = true)
    {
        otherSlots ??= [];
        var r = new Random();
        //Pick a random shift in the passed shift list
        //var pickedShift = shifts[r.Next(shifts.Count - 1)];

        var availableRooms = new List<RoomModel>();

        var timeSlots = new List<(DateOnly, Shift)>();
        //var dates = new HashSet<DateOnly>();
        var currentDate = startWeek;
        var slotCount = 0;

        var daysFrame = new List<(DayOfWeek, Shift)>();
        var shiftCount = Enum.GetValues<Shift>().Length;
        foreach (var frame in frames)
            if (!daysFrame.Any(d => d.Item1 == frame.Item1))
                daysFrame.Add(frame);

        var daysFrameCount = daysFrame.Count;
        while (daysFrameCount < Math.Min(7, level.SlotPerWeek))
        {
            var randomDay = r.Next(6);
            while (daysFrame.Any(d => (int)d.Item1 == randomDay)) randomDay = r.Next(6);
            daysFrame.Add(((DayOfWeek)randomDay,
                daysFrameCount == 0 ? (Shift)r.Next(shiftCount - 1) : daysFrame[0].Item2));
            daysFrameCount++;
        }

        while (slotCount < level.TotalSlots)
        {
            foreach (var frame in daysFrame)
            {
                var date = currentDate.AddDays((int)frame.Item1);

                if (!dayOffs.Any(dayOff =>
                        date.ToDateTime(TimeOnly.MinValue) >= dayOff.StartTime &&
                        date.ToDateTime(TimeOnly.MaxValue) <= dayOff.EndTime))
                {
                    //dates.Add(date);
                    timeSlots.Add((date, frame.Item2));
                    slotCount++;

                    if (slotCount >= level.TotalSlots)
                        break;
                }
            }

            currentDate = currentDate.AddDays(7); // Move to the next week
        }

        //Check available rooms -- If not, iterate
        availableRooms = await _serviceFactory.RoomService.GetAvailableRooms(timeSlots, otherSlots);
        if (availableRooms.Count == 0)
        {
            if (breakWhenFail)
                throw new ConflictException(
                    "Unable to complete arranging classes! No available rooms found! Consider different time or switch to manual scheduling");

            return ([], daysFrame);
        }


        //Pick a room
        var room = availableRooms[r.Next(availableRooms.Count - 1)];
        return ([
            .. timeSlots.Select(ts => new SlotModel
            {
                Id = Guid.NewGuid(),
                Shift = ts.Item2,
                Date = ts.Item1,
                RoomId = room.Id
            })
        ], daysFrame);
    }
}