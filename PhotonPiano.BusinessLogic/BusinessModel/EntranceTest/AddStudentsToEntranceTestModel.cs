namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;

public record AddStudentsToEntranceTestModel
{
    public required List<string> StudentIds { get; init; }
}