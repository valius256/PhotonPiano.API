namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;

public record EntranceTestStudentModel
{
    public required Guid Id { get; init; }
    public required string StudentFirebaseId { get; init; }
    public Guid? EntranceTestId { get; init; }
    
    public decimal? BandScore { get; init; }
    
    public int? Rank { get; init; }

    public int? Year { get; init; }
    
    public bool IsScoreAnnounced { get; init; } = false;
    
    public string? InstructorComment { get; init; }
}