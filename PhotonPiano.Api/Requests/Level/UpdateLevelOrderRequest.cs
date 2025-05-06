using PhotonPiano.BusinessLogic.BusinessModel.Level;

namespace PhotonPiano.Api.Requests.Level
{
    public class UpdateLevelOrderRequest
    {
        public List<LevelOrderModel> LevelOrders { get; set; } = []; // Must include all level
    }
}
