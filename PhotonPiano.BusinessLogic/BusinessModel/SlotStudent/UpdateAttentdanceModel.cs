namespace PhotonPiano.BusinessLogic.BusinessModel.SlotStudent;

public record UpdateAttentdanceModel
{
    public required Guid SlotId { get; init; }
    public List<string> StudentAttentIds { get; init; } = [];
    public List<string>? StudentAbsentIds { get; init; }
}