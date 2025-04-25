namespace PhotonPiano.DataAccess.Models.Entity;

public class StudentClass : BaseEntityWithId
{
    public Guid ClassId { get; set; }
    public required string StudentFirebaseId { get; set; }
    public required string CreatedById { get; set; }
    public string? UpdateById { get; set; }
    public string? DeletedById { get; set; }
    public string? CertificateUrl { get; set; }
    public decimal? AttendancePercentage { get; set; }
    
    public bool IsPassed { get; set; }
    public decimal? GPA { get; set; }
    public string? InstructorComment { get; set; }


    // reference
    public virtual Class Class { get; set; } = default!;
    public virtual Account Student { get; set; } = default!;

    public virtual ICollection<StudentClassScore> StudentClassScores { get; set; } = new List<StudentClassScore>();
    public virtual ICollection<Tuition> Tutions { get; set; } = new List<Tuition>();
    public virtual Account CreatedBy { get; set; } = default!;
    public virtual Account UpdateBy { get; set; } = default!;
    public virtual Account DeletedBy { get; set; } = default!;
}