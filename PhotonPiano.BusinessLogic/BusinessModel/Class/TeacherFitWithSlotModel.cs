using PhotonPiano.BusinessLogic.BusinessModel.Slot;

namespace PhotonPiano.BusinessLogic.BusinessModel.Class;

public record TeacherFitWithSlotModel
{
    public string TeacherId { get; init; }
    public string? TeacherName { get; init; }
    public List<SlotWithInforModel> Slots { get; init; } = new();

    public int TotalSlots { get; init; }
}