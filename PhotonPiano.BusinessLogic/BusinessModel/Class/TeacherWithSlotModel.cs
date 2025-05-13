namespace PhotonPiano.BusinessLogic.BusinessModel.Class;

public record TeacherWithSlotModel
{
    public List<TeacherFitWithSlotModel> TeacherWithSlots { get; init; } = new();
}