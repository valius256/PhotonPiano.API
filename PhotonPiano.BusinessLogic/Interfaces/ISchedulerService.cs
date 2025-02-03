using PhotonPiano.BusinessLogic.BusinessModel.Utils;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ISchedulerService
{
    Graph<EntranceTest> BuildEntranceTestsConflictGraph(List<EntranceTest> entranceTests);

    List<TimeSlot> GenerateValidTimeSlots(DateTime startDate, DateTime endDate, List<DateTime> holidays, params List<Shift> shiftOptions);

    Task<List<EntranceTest>> AssignTimeSlotsToEntranceTests(List<EntranceTest> entranceTests, Graph<EntranceTest> graph,
        DateTime startDate, DateTime endDate,
        List<TimeSlot> validSlots);
}