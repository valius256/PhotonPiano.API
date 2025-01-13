using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTestStudent;
using PhotonPiano.BusinessLogic.BusinessModel.Room;

namespace PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;

public record EntranceTestDetailModel : EntranceTestModel
{
    public RoomModel Room { get; init; }

    public AccountModel CreatedBy { get; init; }

    public AccountModel Instructor { get; init; }
    public List<EntranceTestStudentModel> EntranceTestStudents { get; init; } = [];
}