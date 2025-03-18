namespace PhotonPiano.BusinessLogic.BusinessModel.Level;

public record LevelModel
{
    public required Guid Id { get; init; }
    
    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public List<string> SkillsEarned { get; init; } = [];

    public int SlotPerWeek { get; init; }

    public int TotalSlots { get; init; }

    public decimal PricePerSlot { get; init; }

    public double MinimumScore { get; init; }

    public bool IsGenreDivided { get; init; }

    public Guid? NextLevelId { get; init; }
}