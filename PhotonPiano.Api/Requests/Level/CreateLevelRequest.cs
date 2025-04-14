using System.ComponentModel.DataAnnotations;

namespace PhotonPiano.Api.Requests.Level;

public record CreateLevelRequest
{
    [Required]
    public required string Name { get; init; }

    [Required]
    public required string Description { get; init; }
    
    [Required]
    public required List<string> SkillsEarned { get; init; } 

    public int SlotPerWeek { get; init; }

    public int TotalSlots { get; init; }

    public decimal PricePerSlot { get; init; }

    public decimal MinimumTheoreticalScore { get; init; }

    public decimal MinimumPracticalScore { get; init; }

    public bool IsGenreDivided { get; init; }

    public Guid? NextLevelId { get; init; }

    public string? ThemeColor { get; init; }
}