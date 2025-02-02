using PhotonPiano.BusinessLogic.BusinessModel.Utils;
using PhotonPiano.BusinessLogic.Interfaces;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Exceptions;

namespace PhotonPiano.BusinessLogic.Services;

public class SchedulerService : ISchedulerService
{
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

    public List<TimeSlot> GenerateValidTimeSlots(DateTime startDate, DateTime endDate, List<DateTime> holidays,
        params List<Shift> shiftOptions)
    {
        List<TimeSlot> validSlots = [];
        DateTime currentDate = startDate;

        var shifts = shiftOptions.Count > 0 ? shiftOptions : Enum.GetValues<Shift>().ToList();

        while (currentDate <= endDate)
        {
            if (!holidays.Contains(currentDate))
            {
                validSlots.AddRange(
                    shifts.Select(shift => new TimeSlot { Date = currentDate, Shift = shift }));
            }

            currentDate = currentDate.AddDays(1);
        }

        return validSlots;
    }


    public List<EntranceTest> AssignTimeSlotsToEntranceTests(List<EntranceTest> entranceTests,
        Graph<EntranceTest> graph,
        List<TimeSlot> validSlots)
    {
        Dictionary<EntranceTest, TimeSlot> testTimeMap = [];

        foreach (var entranceTest in entranceTests)
        {
            HashSet<TimeSlot> usedSlots = [];

            // Check used TimeSlots by conflicting tests
            if (!graph.Edges.TryGetValue(entranceTest, out var value))
            {
                continue;
            }

            foreach (var neighbor in value.Where(neighbor => testTimeMap.ContainsKey(neighbor)))
            {
                usedSlots.Add(testTimeMap[neighbor]);
            }

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

        return entranceTests;
    }
}