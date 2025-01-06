using System.ComponentModel.DataAnnotations;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.Models.Entity;

public class EntranceTestStudent : BaseEntityWithId
{
    public required string LearnerFirebaseId { get; set; }
    public required Guid EntranceTestId { get; set; } 
    public decimal? BandScore { get; set; }
    [Range(1,5)]
    public int? Rank { get; set; }
    public int? Year { get; set; } 
    public bool IsScoreAnnounced { get; set; } = false;
    
    // reference 
    public virtual Account LearnerAccount { get; set; }
    public virtual EntranceTest EntranceTest { get; set; } 
}