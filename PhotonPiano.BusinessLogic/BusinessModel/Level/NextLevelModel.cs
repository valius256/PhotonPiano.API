namespace PhotonPiano.BusinessLogic.BusinessModel.Level;

public record NextLevelModel
{
    public Guid Id { get; init; }
    public string? ThemeColor { get; init; }
    public string? Description { get; init; }
}