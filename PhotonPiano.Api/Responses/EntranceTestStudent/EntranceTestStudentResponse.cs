namespace PhotonPiano.Api.Responses.EntranceTestStudent;

public record EntranceTestStudentResponse
{
    public required Guid Id { get; init; }
    public required string StudentFirebaseId { get; init; }
    public required Guid EntranceTestId { get; init; }
    public decimal? BandScore { get; init; }
    public int? Rank { get; init; }
    public int? Year { get; init; }
    public Guid? LevelId { get; init; }
    public bool IsScoreAnnounced { get; init; }
}