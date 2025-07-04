namespace PhotonPiano.DataAccess.Models.Entity;

public class EntranceTestStudent : BaseEntityWithId
{
    public required string StudentFirebaseId { get; set; }
    public Guid? EntranceTestId { get; set; }
    public decimal? BandScore { get; set; }
    public string? FullName { get; set; }
    public Guid? LevelId { get; set; }
    public double? TheoraticalScore { get; set; }

    public int? Year { get; set; }
    
    public bool IsScoreAnnounced { get; set; } = false;
    public string? InstructorComment { get; set; }

    public DateTime? LevelAdjustedAt { get; set; }
    public required string CreatedById { get; set; }
    public string? UpdateById { get; set; }
    public string? DeletedById { get; set; }

    // reference 
    public virtual Account Student { get; set; } = default!;
    public virtual Account CreateBy { get; set; } = default!;
    public virtual Account UpdateBy { get; set; } = default!;
    public virtual Account DeletedBy { get; set; } = default!;
    public virtual Level? Level { get; set; }
    public virtual EntranceTest EntranceTest { get; set; } = default!;

    public virtual ICollection<EntranceTestResult> EntranceTestResults { get; set; } = new List<EntranceTestResult>();
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}