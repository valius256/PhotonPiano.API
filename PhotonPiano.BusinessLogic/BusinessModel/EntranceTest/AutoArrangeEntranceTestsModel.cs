namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;

public record AutoArrangeEntranceTestsModel
{
    public required List<string> StudentIds { get; init; }

    public required DateTime StartDate { get; init; }
    
    public required DateTime EndDate { get; init; }

    public void Deconstruct(out List<string> studentIds, out DateTime startDate, out DateTime endDate)
    {
        studentIds = StudentIds;
        startDate = StartDate;
        endDate = EndDate;
    }
}