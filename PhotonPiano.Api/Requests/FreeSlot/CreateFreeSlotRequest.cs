using PhotonPiano.BusinessLogic.BusinessModel.FreeSlot;
using PhotonPiano.DataAccess.Models.Enum;

namespace PhotonPiano.Api.Requests.FreeSlot
{
    public class CreateFreeSlotRequest
    {
        public List<CreateFreeSlotModel> CreateFreeSlotModels { get; set; } = [];
    }
}
