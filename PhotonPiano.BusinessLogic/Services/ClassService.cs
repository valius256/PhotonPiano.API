using Microsoft.EntityFrameworkCore;
using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.Room;
using PhotonPiano.BusinessLogic.BusinessModel.SystemConfig;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
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

    public async Task<ClassModel> GetClassByUserFirebaseId(string userFirebaseId)
    {
        var result = await _unitOfWork.ClassRepository.FindSingleProjectedAsync<ClassModel>(x =>
            x.InstructorId == userFirebaseId ||
            Enumerable.Any(x.StudentClasses, sc => sc.StudentFirebaseId == userFirebaseId));
        if (result == null) throw new NotFoundException("Class not found");
        return result;
    }

    public async Task<PagedResult<ClassModel>> GetPagedClasses(QueryClassModel queryClass)
    {
        var (page, pageSize, sortColumn, orderByDesc,
            classStatus, level, keyword, isScorePublished, teacherId, studentId) = queryClass;

        var likeKeyword = queryClass.GetLikeKeyword();

        var result = await _unitOfWork.ClassRepository.GetPaginatedWithProjectionAsync<ClassModel>(
            page, pageSize, sortColumn, orderByDesc,
            expressions:
            [
                q => classStatus.Count == 0 || classStatus.Contains(q.Status),
                q => level.Count == 0 || level.Contains(q.Level ?? Level.Beginner),
                q => !isScorePublished.HasValue || q.IsScorePublished == isScorePublished,
                q => teacherId == null || q.InstructorId == teacherId,
                q => studentId == null || Enumerable.Any(q.StudentClasses, sc => sc.StudentFirebaseId == studentId),
                q =>
                    string.IsNullOrEmpty(keyword) ||
                    EF.Functions.ILike(EF.Functions.Unaccent(q.Name), likeKeyword)
            ]
        );
        // Fetch the class capacity
        var capacity =
            int.Parse((await _serviceFactory.SystemConfigService.GetConfig("Sĩ số lớp tối đa")).ConfigValue ?? "0");

        // Fetch students of the classes in one query
        var classIds = result.Items.Select(c => c.Id).ToList();
        var studentClasses = await _unitOfWork.StudentClassRepository
            .FindAsync(sc => classIds.Contains(sc.ClassId));

        var countDictionary = studentClasses
            .GroupBy(c => c.ClassId)
            .Select(g => new { ClassId = g.Key, Count = g.Count() })
            .ToDictionary(c => c.ClassId, c => c.Count);


        // Update items efficiently
        result.Items = result.Items.Select(item => item with
        {
            Capacity = capacity,
            StudentNumber = countDictionary.TryGetValue(item.Id, out var count) ? count : 0
        }).ToList();


        return result;
    }

    public async Task<List<ClassModel>> AutoArrangeClasses(ArrangeClassModel arrangeClassModel)
    {
        /*===============================
         * 1. Fill students in a class
         * 2. Assign a schedule, check for available rooms (if not, pick again)
         * 3. Create slots based on the selected time and places
         * 4. Export the results
        =================================*/
        //Get awaited students
        var students = await _unitOfWork.AccountRepository.FindAsync(a => a.Role == Role.Student && a.StudentStatus == StudentStatus.WaitingForClass);

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

            int numberOfStudentEachClass = 0;

            var validNumbersOfClasses = GetNumberOfClasses(minStudents, studentsOfLevel.Count, maxStudents);
            int numberOfClasses = validNumbersOfClasses.Count == 0 ? 1 : validNumbersOfClasses[0];

            if (validNumbersOfClasses.Count != 0)
            {
                numberOfStudentEachClass = (int) Math.Ceiling((studentsOfLevel.Count * 1.0) / numberOfClasses);
            } else
            {
                numberOfStudentEachClass = maxStudents;
                numberOfClasses = (int)Math.Floor((studentsOfLevel.Count * 1.0) / maxStudents);
            }
            //Great! With number of students each class and number of classes, we can easily fill in students
            for (var i = 0; i < numberOfClasses; i++)
            {
                var selectedStudents = PickRandomFromList(ref studentsOfLevel, numberOfStudentEachClass);
                classes.Add(new CreateClassAutoModel
                {
                    Id = Guid.NewGuid(),
                    Level = level,
                    Name = $"LEVEL{(int)level + 1}_{i}_{DateTime.Now.Month}{DateTime.Now.Year}", //There is a naming rule, handle later
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
            var config = await _serviceFactory.SystemConfigService.GetSystemConfigValueBaseOnLevel(((int) level) + 1);
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

            //Check available rooms -- If not, iterate
            int maxAttempt = 100, attempt = 1;
            var availableRooms = new List<RoomModel>();
            while (availableRooms.Count == 0 && attempt < maxAttempt)
            {
                availableRooms = await _serviceFactory.RoomService.GetAvailableRooms(pickedShift, dayFrames, arrangeClassModel.StartWeek, levelConfig.TotalSlot);
                attempt++;
                if (attempt >= maxAttempt)
                {
                    throw new ConflictException("Unable to complete arranging classes! No available rooms found! Consider different start week or change the shift range");
                }
            }
            //Pick a room
            var room = availableRooms[random.Next(availableRooms.Count - 1)];


            //3. CREATE THE SLOTS
            
        }

        throw new NotImplementedException();
    }
    /// <summary>
    /// Get random days of the week ensure that each day is at least 1 day apart if possible
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private List<DayOfWeek> GetRandomDays(int n)
    {
        if (n < 1 || n > 7)
            throw new ArgumentException("n must be between 1 and 7", nameof(n));

        Random random = new();
        List<DayOfWeek> allDays = [.. Enum.GetValues<DayOfWeek>()];

        List<DayOfWeek> selectedDays = [];

        while (selectedDays.Count < n)
        {
            var availableDays = allDays.Where(d => !selectedDays.Any(s => Math.Abs((int)s - (int)d) == 1)).ToList();

            // If spacing is no longer possible, allow selection from all remaining days
            if (availableDays.Count == 0)
                availableDays = allDays.Except(selectedDays).ToList();

            if (availableDays.Count == 0)
                break; // Safety check

            var chosenDay = availableDays[random.Next(availableDays.Count)];
            selectedDays.Add(chosenDay);
        }

        return selectedDays;
    }
    /// <summary>
    /// Pick randomly n elements from a list and remove it from the list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="n"></param>
    /// <returns></returns>
    private List<T> PickRandomFromList<T>(ref List<T> list, int n)
    {
        Random rand = new Random();
        List<T> selected = [];

        if (n > list.Count)
            n = list.Count; // Ensure we don't remove more elements than available

        for (int i = 0; i < n; i++)
        {
            int index = rand.Next(list.Count); // Get random index
            selected.Add(list[index]); // Add to selected list
            list.RemoveAt(index); // Remove from original list
        }

        return selected;
    }

    private List<int> GetNumberOfClasses(double A, double B, double C)
    {
        List<int> validNs = [];

        // Solve for lower and upper bounds of N
        double minN = B / C; // Derived from B/N <= C -> N >= B/C
        double maxN = B / A; // Derived from A <= B/N -> N <= B/A

        // Convert bounds to integer range
        int start = (int)Math.Ceiling(minN); // Smallest integer >= minN
        int end = (int)Math.Floor(maxN);     // Largest integer <= maxN

        // Collect valid integer values
        for (int N = start; N <= end; N++)
        {
            validNs.Add(N);
        }

        return validNs;
    }
}