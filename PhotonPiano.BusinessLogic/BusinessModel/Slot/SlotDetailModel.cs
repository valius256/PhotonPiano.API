using PhotonPiano.BusinessLogic.BusinessModel.Class;
using PhotonPiano.BusinessLogic.BusinessModel.Room;
using PhotonPiano.BusinessLogic.BusinessModel.SlotStudent;

namespace PhotonPiano.BusinessLogic.BusinessModel.Slot;

public record SlotDetailModel : SlotModel
{
    public RoomModel Room { get; init; } = default!;
    public ClassModel Class { get; init; } = default!;

    public List<SlotStudentModel> SlotStudents { get; init; } = [];

    public int NumberOfStudents => SlotStudents.Count;
}