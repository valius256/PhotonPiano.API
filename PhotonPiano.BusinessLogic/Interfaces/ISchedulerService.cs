using PhotonPiano.BusinessLogic.BusinessModel.Utils;
using PhotonPiano.DataAccess.Models.Entity;
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Enums;

namespace PhotonPiano.BusinessLogic.Interfaces;

public interface ISchedulerService
{
    Graph<EntranceTest> BuildEntranceTestsConflictGraph(List<EntranceTest> entranceTests);

    Task<List<TimeSlot>> GenerateValidTimeSlots(List<EntranceTest> existingTests, DateTime startDate, DateTime endDate,
        List<DateTime> holidays, params List<Shift> shiftOptions);

    Task<List<EntranceTest>> AssignTimeSlotsToEntranceTests(List<EntranceTest> assigningEntranceTests, Graph<EntranceTest> graph,
        DateTime startDate, DateTime endDate,
        List<TimeSlot> validSlots, List<EntranceTest> existingEntranceTests);

    Task<List<EntranceTest>> AssignInstructorsToEntranceTests(List<EntranceTest> assigningEntranceTests,
        List<TimeSlot> validSlots);
}