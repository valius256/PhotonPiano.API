using PhotonPiano.BusinessLogic.BusinessModel.Slot;
using PhotonPiano.BusinessLogic.BusinessModel.Utils;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Abstractions;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Enums;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services;

public class SchedulerService : ISchedulerService
{
    private readonly IUnitOfWork _unitOfWork;

    public SchedulerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Graph<EntranceTest> BuildEntranceTestsConflictGraph(List<EntranceTest> entranceTests)
    {
        Graph<EntranceTest> graph = new();

        for (int i = 0; i < entranceTests.Count; i++)
        {
            var test1 = entranceTests[i];

            for (int j = i + 1; j < entranceTests.Count; j++)
            {
                var test2 = entranceTests[j];

                if (!test1.EntranceTestStudents.Intersect(test2.EntranceTestStudents).Any())
                {
                    continue;
                }

                if (!graph.Edges.ContainsKey(test1))
                {
                    graph.Edges[test1] = [];
                }

                if (!graph.Edges.ContainsKey(test2))
                {
                    graph.Edges[test2] = [];
                }

                graph.Edges[test1].Add(test2);
                graph.Edges[test2].Add(test1);
            }
        }

        return graph;
    }

    public async Task<List<TimeSlot>> GenerateValidTimeSlots(List<EntranceTest> existingTests, DateTime startDate,
        DateTime endDate, List<DateTime> holidays,
        params List<Shift> shiftOptions)
    {
        List<TimeSlot> validSlots = [];
        DateTime currentDate = startDate;

        var shifts = shiftOptions.Count > 0 ? shiftOptions : Enum.GetValues<Shift>().ToList();

        while (currentDate <= endDate)
        {
            if (!holidays.Contains(currentDate))
            {
                foreach (var shift in shifts)
                {
                    // 🔹 Check if the current date & shift is already taken
                    bool isSlotTaken = existingTests.Any(et =>
                        et.Date == DateOnly.FromDateTime(currentDate) && et.Shift == shift);

                    if (!isSlotTaken)
                    {
                        validSlots.Add(new TimeSlot { Date = currentDate, Shift = shift });
                    }
                }
            }

            currentDate = currentDate.AddDays(1);
        }

        return validSlots;
    }


    public async Task<List<EntranceTest>> AssignTimeSlotsToEntranceTests(List<EntranceTest> assigningEntranceTests,
        Graph<EntranceTest> graph,
        DateTime startDate, DateTime endDate,
        List<TimeSlot> validSlots,
        List<EntranceTest> existingEntranceTests)
    {
        Dictionary<EntranceTest, TimeSlot> testTimeMap = [];

        foreach (var entranceTest in assigningEntranceTests)
        {
            HashSet<TimeSlot> usedSlots = [];

            // 🔹 Get conflicting tests if they exist
            if (graph.Edges.TryGetValue(entranceTest, out var conflictingTests))
            {
                foreach (var neighbor in conflictingTests.Where(neighbor => testTimeMap.ContainsKey(neighbor)))
                {
                    usedSlots.Add(testTimeMap[neighbor]);
                }
            }

            // 🔹 Check existing tests in the database that share the same room and date
            foreach (var existingTest in existingEntranceTests)
            {
                if (existingTest.RoomId == entranceTest.RoomId && existingTest.Date == entranceTest.Date)
                {
                    usedSlots.Add(new TimeSlot
                    { Date = existingTest.Date.ToDateTime(TimeOnly.MinValue), Shift = existingTest.Shift });
                }
            }

            // 🔹 Assign the first available valid slot
            var assignedSlot = validSlots.FirstOrDefault(slot => !usedSlots.Contains(slot));

            if (assignedSlot is not null)
            {
                testTimeMap[entranceTest] = assignedSlot;
                entranceTest.Shift = assignedSlot.Shift;
                entranceTest.Date = DateOnly.FromDateTime(assignedSlot.Date);
            }
            else
            {
                throw new BadRequestException("No available time slots for test scheduling.");
            }
        }

        return assigningEntranceTests;
    }


    public async Task<List<EntranceTest>> AssignInstructorsToEntranceTests(List<EntranceTest> assigningEntranceTests,
        List<TimeSlot> validSlots)
    {
        var instructors = await _unitOfWork.AccountRepository.FindAsync(a => a.Role == Role.Instructor
                                                                             && a.Status == AccountStatus.Active,
            hasTrackings: false);

        if (instructors.Count == 0)
        {
            throw new BadRequestException("All lecturers are busy, please try again later.");
        }

        //Get all booked slot
        var bookedSlots =
            await _unitOfWork.SlotRepository.FindProjectedAsync<SlotWithClassModel>(
                s => s.Status != SlotStatus.NotStarted, hasTrackings: false);

        var assignedTests = new List<EntranceTest>();
        var unassignedTests = new List<EntranceTest>(); // Track tests that need rescheduling

        foreach (var entranceTest in assigningEntranceTests)
        {
            // Find a lecturer who is free at the test's date and shift
            var availableLecturer = instructors.FirstOrDefault(l =>
                !bookedSlots.Any(s =>
                    s.Class.InstructorId == l.AccountFirebaseId &&
                    s.Date == entranceTest.Date &&
                    s.Shift == entranceTest.Shift
                )
            );

            if (availableLecturer is not null)
            {
                entranceTest.InstructorId = availableLecturer.AccountFirebaseId;
                entranceTest.InstructorName = !string.IsNullOrEmpty(availableLecturer.FullName) ? availableLecturer.FullName : availableLecturer.Email;

                // Mark this slot as booked
                bookedSlots.Add(new SlotWithClassModel
                {
                    Id = Guid.CreateVersion7(),
                    Date = entranceTest.Date,
                    Shift = entranceTest.Shift,
                    Status = SlotStatus.NotStarted
                });

                assignedTests.Add(entranceTest);
            }
            else
            {
                // No lecturer found → add test to reschedule list
                unassignedTests.Add(entranceTest);
            }
        }

        //Try rescheduling unassigned tests
        foreach (var test in unassignedTests.ToList())
        {
            var alternativeSlot = validSlots.FirstOrDefault(slot =>
                !assigningEntranceTests.Any(et => et.Date == DateOnly.FromDateTime(slot.Date) && et.Shift == slot.Shift)
                && !bookedSlots.Any(s => s.Date == DateOnly.FromDateTime(slot.Date) && s.Shift == slot.Shift)
            );

            if (alternativeSlot is not null)
            {
                // Update test to new shift
                test.Date = DateOnly.FromDateTime(alternativeSlot.Date);
                test.Shift = alternativeSlot.Shift;

                // Retry lecturer assignment
                var availableLecturer = instructors.FirstOrDefault(a =>
                    !bookedSlots.Any(s =>
                        s.Class.InstructorId == a.AccountFirebaseId &&
                        s.Date == test.Date &&
                        s.Shift == test.Shift
                    )
                );

                if (availableLecturer is not null)
                {
                    test.InstructorId = availableLecturer.AccountFirebaseId;
                    test.InstructorName = availableLecturer.FullName;

                    assignedTests.Add(test);

                    unassignedTests.Remove(test);

                    // Mark this new slot as booked
                    bookedSlots.Add(new SlotWithClassModel
                    {
                        Id = Guid.CreateVersion7(),
                        Date = test.Date,
                        Shift = test.Shift,
                        Status = SlotStatus.NotStarted
                    });

                    continue;
                }
            }
        }

        if (unassignedTests.Count != 0)
        {
            // If rescheduling fails, notify admin or handle manually
            throw new BadRequestException(
                $"No available lecturer or alternative slot for entrance test {unassignedTests.Select(t => t.Id).ToArray()}.");
        }

        return assignedTests;
    }
}