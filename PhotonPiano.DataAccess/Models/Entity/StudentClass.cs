using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.Models.Entity;

public class StudentClass : BaseEntityWithId
{
    public Guid ClassId { get; set; }
    public string? StudentFirebaseId { get; set; }
    public ClassStatus Status { get; set; } = ClassStatus.NotStarted;
    public bool IsFinished { get; set; } = false;
    public bool IsScorePublished { get; set; } = false;
    public required string CreatedById { get; set; }
    public string? UpdateById { get; set; }
    public string? DeletedById { get; set; }
    public string? CertificateUrl { get; set; }

    // reference
    public virtual Class Class { get; set; } = default!;
    public virtual Account Student { get; set; } = default!;

    public virtual ICollection<StudentClassScore> StudentClassScores { get; set; } = new List<StudentClassScore>();
    public virtual ICollection<Tution> Tutions { get; set; } = new List<Tution>();
    public virtual Account CreatedBy { get; set; } = default!;
    public virtual Account UpdateBy { get; set; } = default!;
    public virtual Account DeletedBy { get; set; } = default!;
}