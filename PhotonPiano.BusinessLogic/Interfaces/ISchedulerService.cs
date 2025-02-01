using PhotonPiano.BusinessLogic.BusinessModel.Utils;
using PhotonPiano.DataAccess.Models.Entity;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ISchedulerService
{
    Graph<EntranceTest> BuildEntranceTestsConflictGraph(List<EntranceTest> entranceTests);

    List<TimeSlot> GenerateValidTimeSlots(DateTime startDate, DateTime endDate, List<DateTime> holidays);

    List<EntranceTest> AssignTimeSlotsToEntranceTests(List<EntranceTest> entranceTests, Graph<EntranceTest> graph,
        List<TimeSlot> validSlots);
}