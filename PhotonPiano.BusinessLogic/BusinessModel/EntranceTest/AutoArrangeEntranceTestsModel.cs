using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Enums;

namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;

public record AutoArrangeEntranceTestsModel
{
    public required List<string> StudentIds { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }
    public List<Shift> ShiftOptions { get; init; } = [];

    public void Deconstruct(out List<string> studentIds, out DateTime startDate, out DateTime endDate, out List<Shift> shiftOptions)
    {
        studentIds = StudentIds;
        startDate = StartDate;
        endDate = EndDate;
        shiftOptions = ShiftOptions;
    }
}