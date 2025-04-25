
using PhotonPiano.DataAccess.Models.Enum;
using PhotonPiano.Shared.Enums;

namespace PhotonPiano.BusinessLogic.BusinessModel.FreeSlot
{
    public class CreateFreeSlotModel
    {
        public DayOfWeek DayOfWeek { get; set; }
        public Shift Shift { get; set; }
    }
}
