namespace PhotonPiano.Api.Responses.EntranceTestStudent;

public record EntranceTestStudentResponse : BaseResponse
{
    public required Guid Id { get; init; }
    
    public required string StudentFirebaseId { get; init; }
    
    public Guid? LevelId { get; init; }
    
    public Guid? EntranceTestId { get; init; }

    public string? FullName { get; init; }

    public decimal? BandScore { get; init; }

    public double? TheoraticalScore { get; init; }
    
    public DateTime? LevelAdjustedAt { get; init; }

    public int? Year { get; init; }

    public bool IsScoreAnnounced { get; init; } = false;

    public string? InstructorComment { get; init; }
}