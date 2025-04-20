namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTestResult;

public record UpdateEntranceTestResultsModel
{
    public string? InstructorComment { get; init; }

    public double? TheoraticalScore { get; init; }
    
    public Guid? LevelId { get; init; }

    public List<UpdateEntranceTestScoreModel> UpdateScoreRequests { get; init; } = [];
}

public record UpdateEntranceTestScoreModel
{
    public required Guid CriteriaId { get; init; }

    public required decimal Score { get; init; }
}