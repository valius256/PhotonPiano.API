using PhotonPiano.BusinessLogic.BusinessModel.Account;
using PhotonPiano.BusinessLogic.BusinessModel.EntranceTest;

namespace PhotonPiano.BusinessLogic.BusinessModel.Room;

public record RoomDetailModel : RoomModel
{
    public AccountSimpleModel? CreatedBy { get; init; }
    public List<EntranceTestModel> EntranceTests { get; set; } = [];
}