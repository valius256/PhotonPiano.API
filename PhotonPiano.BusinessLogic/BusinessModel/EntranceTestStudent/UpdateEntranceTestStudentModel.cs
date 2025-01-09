namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;

public class UpdateEntranceTestStudentModel
{
    public required string LearnerFirebaseId { get; init; }
    public required Guid EntranceTestId { get; init; } 
    public decimal? BandScore { get; init; }
    public int? Rank { get; init; }
    public int? Year { get; init; } 
    public bool IsScoreAnnounced { get; init; }
}