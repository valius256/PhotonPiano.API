namespace PhotonPiano.BusinessLogic.BusinessModel.Level;

public record CreateLevelModel
{
    public required string Name { get; init; }
    
    public required string Description { get; init; }

    public required List<string> SkillsEarned { get; init; } 

    public int SlotPerWeek { get; init; }

    public int TotalSlots { get; init; }

    public decimal PricePerSlot { get; init; }

    public decimal MinimumTheoreticalScore { get; init; }

    public decimal MinimumPracticalScore { get; init; }
    public decimal MinimumGPA { get; init; }

    public bool IsGenreDivided { get; init; }

    public Guid? NextLevelId { get; init; }
    
    public string? ThemeColor { get; init; }
    
    public bool RequiresEntranceTest { get; init; }
}