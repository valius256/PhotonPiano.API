namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;

public record UpdateStudentsEntranceTestResultsModel
{
    public required List<UpdateStudentEntranceTestResultsModel> UpdateRequests { get; init; }
}

public record UpdateStudentEntranceTestResultsModel
{
    public required string StudentId { get; init; }

    public required double TheoraticalScore { get; init; }

    public string? InstructorComment { get; init; }

    public required List<UpdateEntranceTestScoreModel> Scores { get; init; }
}

public record UpdateEntranceTestScoreModel
{
    public required Guid CriteriaId { get; init; }

    public required decimal Score { get; init; }
}