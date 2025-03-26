namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;

public record EntranceTestStudentModel
{
    public required Guid Id { get; init; }
    public required string StudentFirebaseId { get; init; }
    public Guid? EntranceTestId { get; init; }

    public string? FullName { get; init; }

    public decimal? BandScore { get; init; }

    public Guid? LevelId { get; init; }

    public double? TheoraticalScore { get; init; }

    public int? Year { get; init; }

    public bool IsScoreAnnounced { get; init; } = false;

    public string? InstructorComment { get; init; }
}