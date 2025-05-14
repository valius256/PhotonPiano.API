namespace PhotonPiano.BusinessLogic.BusinessModel.Level;

public record LevelModel : BaseModel
{
    public required Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public List<string> SkillsEarned { get; init; } = [];

    public int SlotPerWeek { get; init; }

    public int TotalSlots { get; init; }
    public decimal PricePerSlot { get; init; }

    public decimal MinimumTheoreticalScore { get; init; }

    public decimal MinimumPracticalScore { get; init; }

    public decimal MinimumGPA { get; set; }

    public bool IsGenreDivided { get; init; }

    public Guid? NextLevelId { get; init; }

    public string? ThemeColor { get; init; }

    public int? EstimateDurationInWeeks { get; init; }

    public decimal? TotalPrice { get; init; }

    public NextLevelModel? NextLevel { get; set; }

    public int? NumberActiveStudentInLevel { get; init; }
}