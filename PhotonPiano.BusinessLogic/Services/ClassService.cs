using Mapster;
using Microsoft.EntityFrameworkCore;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.Room;
using PhotonPiano.BusinessLogic.BusinessModel.Slot;
using PhotonPiano.BusinessLogic.BusinessModel.SystemConfig;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.DataAccess.Models.Paging;
using PhotonPiano.Shared.Exceptions;

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
        var classDetail = await _unitOfWork.ClassRepository.FindSingleProjectedAsync<ClassDetailModel>(c => c.Id == id);
        if (classDetail is null) throw new NotFoundException("Class not found");
        return classDetail;
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
                q => level.Count == 0 || level.Contains(q.Level),
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
        var levelConfigs = new List<GetSystemConfigOnLevelModel>
        {
            await _serviceFactory.SystemConfigService.GetSystemConfigValueBaseOnLevel(1),
            await _serviceFactory.SystemConfigService.GetSystemConfigValueBaseOnLevel(2),
            await _serviceFactory.SystemConfigService.GetSystemConfigValueBaseOnLevel(3),
            await _serviceFactory.SystemConfigService.GetSystemConfigValueBaseOnLevel(4),
            await _serviceFactory.SystemConfigService.GetSystemConfigValueBaseOnLevel(5)
        };

        // Perform the paged query with projections
        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(q => new ClassModel
            {
                Id = q.Id,
                Name = q.Name,
                Level = q.Level,
                Status = q.Status,
                InstructorId = q.InstructorId,
                IsScorePublished = q.IsScorePublished,
                Capacity = capacity,
                CreatedById = q.CreatedById,
                // Aggregate counts directly in SQL
                StudentNumber = q.StudentClasses.Count(),
                TotalSlots = q.Slots.Count()
            })
            .ToListAsync();

        result = result.Select(item => item with
        {
            RequiredSlots = levelConfigs[(int)item.Level].TotalSlot
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


        //Get awaited students
        var students = await _unitOfWork.AccountRepository.FindAsync(a =>
            a.Role == Role.Student && a.StudentStatus == StudentStatus.WaitingForClass);

        var maxStudents =
            int.Parse((await _serviceFactory.SystemConfigService.GetConfig("Sĩ số lớp tối đa")).ConfigValue ?? "0");
        var minStudents =
            int.Parse((await _serviceFactory.SystemConfigService.GetConfig("Sĩ số lớp tối thiểu")).ConfigValue ?? "0");

        var classes = new List<CreateClassAutoModel>();

        //1. FILL IN STUDENTS
        //With each level, fill the students
        foreach (var level in Enum.GetValues<Level>())
        {
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
                    Level = level,
                    Name =
                        $"LEVEL{(int)level + 1}_{i}_{DateTime.Now.Month}{DateTime.Now.Year}", //There is a naming rule, handle later
                    StudentIds = selectedStudents.Select(s => s.AccountFirebaseId).ToList()
                });
            }
        }

        //2. IT's SCHEDULE TIME! 
        Random random = new();
        //Get config values
        var levelConfigs = new List<GetSystemConfigOnLevelModel>();
        foreach (var level in Enum.GetValues<Level>())
        {
            var config = await _serviceFactory.SystemConfigService.GetSystemConfigValueBaseOnLevel((int)level + 1);
            levelConfigs.Add(config);
        }

        //With each class, we will pick a random schedule for it!
        foreach (var classDraft in classes)
        {
            var levelConfig = levelConfigs[(int)classDraft.Level];

            //Pick n day(s) of the week based on the config
            var dayFrames = GetRandomDays(levelConfig.NumOfSlotInWeek);

            //Pick a random shift in the passed shift list
            var pickedShift = arrangeClassModel.AllowedShifts[random.Next(arrangeClassModel.AllowedShifts.Count - 1)];

            int maxAttempt = 100, attempt = 1; //Avoid infinite loop
            var availableRooms = new List<RoomModel>();


            // Generate all required dates efficiently
            //TODO : Consider day-offs
            var dates = Enumerable.Range(0, levelConfig.TotalSlot)
                .Select(i =>
                    arrangeClassModel.StartWeek.AddDays(i / dayFrames.Count * 7 + (int)dayFrames[i % dayFrames.Count]))
                .ToHashSet();

            //Check available rooms -- If not, iterate
            while (availableRooms.Count == 0 && attempt < maxAttempt)
            {
                availableRooms = await _serviceFactory.RoomService.GetAvailableRooms(pickedShift, dates);
                attempt++;
                if (attempt >= maxAttempt)
                    throw new ConflictException(
                        "Unable to complete arranging classes! No available rooms found! Consider different start week or change the shift range");
            }

            //Pick a room
            var room = availableRooms[random.Next(availableRooms.Count - 1)];

            classDraft.Slots.AddRange(dates.Select(d => new CreateSlotThroughArrangementModel
            {
                Date = d,
                Shift = pickedShift,
                RoomId = room.Id
            }).ToList());
        }

        //4. Now save them to database
        var result = await SaveClasses(classes, students, userId);


        return result;
    }

    private async Task<List<ClassModel>> SaveClasses(List<CreateClassAutoModel> classes, List<Account> students,
        string userId)
    {
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            //Create classes
            var mappedClasses = classes.Adapt<List<Class>>();
            mappedClasses.ForEach(c =>
            {
                c.CreatedById = userId;
                c.IsPublic = false;
                c.IsScorePublished = false;
                c.Status = ClassStatus.NotStarted;
            });
            await _unitOfWork.ClassRepository.AddRangeAsync(mappedClasses.Select(c =>
            {
                c.Slots = [];
                return c;
            }).ToList());

            //Create StudentClasses
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

            //Create Slots
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

            //Create studentSlots
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

            //Change student status
            await _unitOfWork.AccountRepository.FindAsQueryable(a =>
                    a.Role == Role.Student && a.StudentStatus == StudentStatus.WaitingForClass)
                .ExecuteUpdateAsync(setters => setters.SetProperty(b => b.StudentStatus, StudentStatus.InClass));

            await _unitOfWork.SaveChangesAsync();

            // Fetch the class capacity
            var capacity =
                int.Parse((await _serviceFactory.SystemConfigService.GetConfig("Sĩ số lớp tối đa")).ConfigValue ?? "0");

            // create 1st tuition for students in class
            if (studentClasses.Any())
                await _serviceFactory.TuitionService.CreateTuitionWhenRegisterClass(studentClasses, slots);


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
}