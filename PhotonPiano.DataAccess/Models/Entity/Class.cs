using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.DataAccess.Models.Entity;

public class Class : BaseEntityWithId
{
    public string? InstructorId { get; set; }
    public ClassStatus Status { get; set; } = ClassStatus.NotStarted;
    public DateOnly StartTime { get; set; } = DateOnly.MaxValue;
    public Level Level { get; set; }
    public bool IsPublic { get; set; }
    public required string Name { get; set; }
    public required string CreatedById { get; set; }
    public bool IsScorePublished { get; set; }
    public string? UpdateById { get; set; }
    public string? DeletedById { get; set; }


    // reference 
    public virtual Account Instructor { get; set; } = default!;
    public virtual Account CreatedBy { get; set; } = default!;
    public virtual Account UpdateBy { get; set; } = default!;
    public virtual Account DeletedBy { get; set; } = default!;
    public virtual ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();
    public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();
}