using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.Level;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.BusinessLogic.BusinessModel.Class;

public record ClassModel
{
    public required Guid Id { get; init; }
    public string? InstructorId { get; init; }
    public string? InstructorName { get; init; }
    public ClassStatus Status { get; init; } = ClassStatus.NotStarted;
    public DateOnly? StartTime { get; init; }
    public DateOnly? EndTime { get; init; }
    public List<string> ClassTime { get; set; } = [];
    public string? ClassDays { get; set; }
    public Guid LevelId { get; init; }
    public bool IsPublic { get; init; }
    public string? ScheduleDescription { get; set; }
    public required string Name { get; init; }
    public required string CreatedById { get; init; }
    public bool IsScorePublished { get; init; }
    public int Capacity { get; set; }
    public int MinimumStudents { get; init; }
    public int RequiredSlots { get; init; }
    public int TotalSlots { get; set; }
    public int StudentNumber { get; init; }
    public string? UpdateById { get; init; }
    public string? DeletedById { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow.AddHours(7);
    public DateTime? UpdatedAt { get; init; }

    public AccountSimpleModel? Instructor { get; init; }

    public LevelModel? Level { get; init; }
}